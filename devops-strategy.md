# Estrategia DevOps — Request Platform (WTW)

Este documento define las directrices operacionales, el flujo de entrega continua (CI/CD) y las políticas de resiliencia para la plataforma modular de solicitudes. La estrategia está diseñada para minimizar la fricción entre desarrolladores, garantizar despliegues sin caídas de servicio (*Zero-Downtime*) y mantener un estándar estricto de seguridad.

---

## 1. Despliegues por Entorno y Promoción del Código

Adoptamos un flujo de promoción lineal de 4 niveles basado en **Trunk-Based Development**, combinando ambientes efímeros de validación con entornos estables de integración y producción.

```
PR-Preview (Slot Efímero) ──> DEV ──> QA ──> PROD
```

| Entorno | Propósito | Trigger de Despliegue | Aprobación Requerida |
|---------|-----------|----------------------|----------------------|
| **PR-Preview** | Pruebas aisladas por feature antes de tocar el código compartido | Pull Request hacia `main` | Ninguna (creación automática en Slot temporal `pr-*`) |
| **DEV** | Integración continua, resolución de conflictos y smoke tests | Merge a `main` | Ninguna (despliegue continuo automático) |
| **QA** | Regresión automatizada y certificación UAT del Product Owner | Promoción tras éxito en DEV | Automática tras suite de integración verde |
| **PROD** | Producción en vivo con intercambio de ranura (*Slot Swap*) | Promoción desde QA | Manual en Azure DevOps: Tech Lead + PO |

### 1.1 Ambientes Efímeros para Pull Requests (Shift-Left)
Para evitar que ramas incompletas rompan el entorno de desarrollo compartido (**DEV**), cada Pull Request aprovisiona de forma dinámica una ranura (*Slot*) en Azure App Service con el nombre `pr-<NUM>`.
* **Validación temprana:** QA y Producto prueban la funcionalidad en vivo desde un dispositivo móvil o navegador web antes del merge.
* **Conciliación:** Si hay múltiples PRs en paralelo, el desarrollador concilia ejecutando `git rebase origin/main`. Al empujar los cambios, su slot efímero se actualiza automáticamente con ambas funcionalidades en convivencia.
* **Teardown automático:** Al completarse o cerrarse el PR, Azure DevOps ejecuta la limpieza del recurso:
  ```bash
  az webapp deployment slot delete --name wtw-request-platform-api-dev --resource-group rg-wtw-request-platform-dev --slot pr-<PR_NUM>
  ```

### 1.2 Reglas de Convivencia en la Rama Principal (`main`)
* Las ramas de feature viven **máximo 2 a 3 días**.
* Todo merge a `main` requiere **PR aprobado + build verde + conciliación con la rama principal**.
* Los despliegues a Producción utilizan **Blue-Green Deployment** mediante Azure App Service Slots (`staging` → `production`). El código calienta motores en `staging`, pasa por health probes y se intercambia al slot productivo en milisegundos sin cortar peticiones activas.

---

## 2. Gestión de Secrets y Configuración

La regla de oro del equipo es absoluta: **cero credenciales en el repositorio de código fuente**. Ningún *connection string*, API key o certificado vive en el código ni en archivos `appsettings.json`.

### 2.1 Arquitectura de Configuración (Azure Key Vault + Managed Identity)
1. **Azure Key Vault:** Única fuente de verdad para secretos (contraseñas de SQL Server, claves JWT).
2. **Managed Identities:** Las aplicaciones en Azure App Service tienen una identidad administrada asignada por el directorio activo (Microsoft Entra ID). No necesitan contraseña para leer el Key Vault; acceden por RBAC con permisos de solo lectura (`Key Vault Secrets User`).
3. **Inyección en Tiempo de Ejecución:** En .NET 8, conectamos el proveedor nativo en `Program.cs`:
   ```csharp
   builder.Configuration.AddAzureKeyVault(
       new Uri($"https://{keyVaultName}.vault.azure.net/"),
       new DefaultAzureCredential());
   ```

### 2.2 Variable Groups y App Configuration
* Las variables no confidenciales (URLs de CORS, entornos, banderas de características) se gestionan mediante **Azure App Configuration** o **Variable Groups** en Azure DevOps vinculados por etapa (`VG-RequestPlatform-DEV`, `VG-RequestPlatform-PROD`).
* En el pipeline YAML, se inyectan dinámicamente al momento del pase de etapa sin alterar el artefacto binario de la aplicación.

---

## 3. Rollback Automático y Resiliencia

Un despliegue fallido no debe requerir una intervención manual de emergencia a medianoche. El sistema está diseñado para autoguarecerse o revertirse de inmediato.

### 3.1 Health Checks y Compuerta Pre-Swap
Antes de ejecutar un intercambio hacia producción, el pipeline golpea el endpoint de salud (`/api/health`) en la ranura de `staging`.
* Si el endpoint responde `200 OK` y verifica conectividad con la base de datos SQL Server → **Se ejecuta el Swap**.
* Si el endpoint responde `500 Internal Server Error` o tarda más de 5 segundos → **El pipeline aborta el despliegue**. El slot productivo jamás se entera del fallo.

### 3.2 Rollback Instantáneo (Swap-Back)
Si un error lógico de negocio pasa los health checks y genera una anomalía en producción tras el despliegue:
* Al utilizar App Service Slots, el código de la versión anterior (estable) permanece ejecutándose en el slot de `staging`.
* El Tech Lead ejecuta un **Swap-Back** desde la consola de Azure o mediante un comando de emergencia, devolviendo la versión anterior a producción en menos de 5 segundos:
  ```bash
  az webapp deployment slot swap --resource-group rg-wtw-request-platform-prod --name wtw-request-platform-api --slot staging --target-slot production
  ```

### 3.3 Migraciones de Base de Datos Evolutivas
Para que el rollback instantáneo funcione, las bases de datos no pueden sufrir rupturas destructivas.
* **Regla Expand and Contract:** Nunca se elimina ni renombra una columna en uso en el mismo paso. Primero se agrega la columna nueva (*Expand*), se despliega el código que soporta ambas, y en el siguiente release se limpia la columna vieja (*Contract*).
* Al utilizar una columna `DynamicData NVARCHAR(MAX)` con validación `ISJSON`, logramos evolucionar la estructura del payload de las solicitudes sin ejecutar costosos bloqueos de esquema DDL en la tabla principal de SQL Server.
