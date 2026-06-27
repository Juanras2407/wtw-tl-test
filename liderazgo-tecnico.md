# Liderazgo Técnico — Respuestas de Escenarios Reales

---

## 1. Un nuevo desarrollador junior está enviando código sin pruebas. ¿Cómo lo abordas?

### Mi proceso de intervención:

**1. Auditoría del Onboarding**
Lo primero que hago es revisar cómo fue su proceso de inducción (*onboarding*). Verifico si se le entregaron las instrucciones claras sobre nuestro flujo de desarrollo, el estándar de pruebas unitarias y las reglas de despliegue. Muchas veces el error ocurre porque el equipo dio por sentado conocimiento que nunca se impartio formalmente.

**2. Retroalimentación 1:1**
Si el onboarding fue el adecuado, me reúno en un uno a uno con el para recordarle el proceso establecido. En lugar de confrontarlo, indago en la raíz del problema: *"Vi que conocías el proceso del onboarding, ¿qué pasó en este PR en particular? ¿Te encontraste con un bloqueo técnico o hubo confusión con los tiempos?"*. Identificar si fue presión o falta de experiencia me permite actuar con mayor eficacia.

**3. Acción de Mentoría**
Si noto vacíos técnicos, me siento con él (o le asigno un senior) para escribir las pruebas unitarias de ese PR juntos. Le muestro cómo abordamos los casos de borde y cómo estructuramos las aserciones, logrando que pierda el miedo al testing.

**4. Medida Preventiva Institucional (Quality Gate)**
Para que esto no se convierta en algo recurrente o en periodicas revisiones de código, implemento una **medida preventiva en el pipeline CI/CD**: un *Quality Gate* en Azure DevOps que bloquee automáticamente cualquier PR que no incluya pruebas unitarias o que disminuya el porcentaje de cobertura.

**5. Seguimiento Periódico**
Realizo un monitoreo recurrente de sus siguientes entregas en los sprints posteriores, reconociendo públicamente en los daily o sincros, cuando sus PRs comiencen a venir con pruebas sólidas y bien hechas.

---

## 2. Dos miembros del equipo tienen enfoques opuestos sobre manejo de errores. ¿Cómo resuelves el conflicto?

### Mi proceso de resolución:

**1. Revisión de Estándares Existentes**
Antes de opinar, reviso nuestra documentación y guías de arquitectura actuales. Evalúo cuál de los dos enfoques cuenta con argumentos más sólidos y está más alineado con el estándar técnico que ya definió la organización.

**2. Sesión de Alineación y Mediación**
Me reúno con ambos desarrolladores, les presento el estándar actual y les explico con claridad técnica quién de los dos está planteando la solución más cercana a nuestras convenciones.

**3. Integración y Acuerdo Mutuo**
En lugar de imponer un ganador que dañe el ego del otro, busco conciliar e **integrar ambas opiniones**. Por ejemplo: podemos usar el patrón propuesto por el Dev A para la lógica interna del dominio, pero capturarlo y transformarlo según el estándar del Dev B en la capa de la API. 

**4. Evolución Formal**
Si el debate evidenció que nuestro estándar se quedó obsoleto, formalizamos el nuevo acuerdo mutuo redactando un documento que se compartira con el equipo de arquitectura. Así, el conflicto individual se convierte en una mejora institucional para todos.

---

## 3. El PO quiere lanzar una funcionalidad en 3 días, pero sabes que requiere al menos una semana. ¿Qué haces?

### Mi estrategia de negociación:

**1. Visibilizar la Complejidad Real y el Contexto**
Le detallo al PO la complejidad técnica del requerimiento desglosada en horas/días reales. Además, le pongo sobre la mesa el mapa completo de las otras actividades y compromisos que el equipo está ejecutando en ese preciso momento dentro del sprint.

**2. Persuadir sobre el Riesgo de Producto**
Le explico con argumentos de negocio que forzar un desarrollo complejo en tiempos inadecuados generará incidentes graves, afectando directamente la estabilidad del producto y la experiencia de los usuarios finales. Le demuestro que lanzar rápido pero roto sale diez veces más caro.

**3. Negociación de Prioridades**
Si tras presentar los riesgos el PO mantiene que la fecha de 3 días es inamovible por un compromiso comercial crítico, aplico una conciliación de prioridades: **solicito la priorización absoluta de este requerimiento sobre todo lo demás**. Detenemos de inmediato las otras tareas menos prioritarias del sprint para volcamos de lleno a esto, o negociamos recortar el alcance funcional para entregar un MVP seguro en 3 días y la funcionalidad completa en la semana siguiente.

---

## 4. ¿Cómo priorizas entre deuda técnica, nuevas funcionalidades y soporte?

### Modelo de Asignación de Capacidad:

**1. Reserva Fija de Capacidad**
En nuestro acuerdo de trabajo con Producto, establecemos una distribución clara para cada sprint:
* **Nuevas Funcionalidades (~60%):** Foco principal para seguir aportando valor comercial al negocio.
* **Deuda Técnica (~25%):** Capacidad reservada inamovible para refactorizar código lento, actualizar librerías y mejorar arquitectura.
* **Soporte e Incidentes (~15%):** Mantenimiento continuo y corrección de bugs operativos.

**2. Gestión Visibilizada**
Las tareas de deuda técnica se gestionan en un tablero de deuda tecnica (linear, notion, azure devops... etc) formal con prioridades claras. Al tener una capacidad técnica fija preaprobada, el equipo no tiene que pedir "permiso" para limpiar el código; es parte natural de nuestro ciclo de trabajo.

---

## 5. ¿Qué es aquello que no negocias como líder técnico?

**1. La Calidad y las Pruebas en los Desarrollos**
No negocio lanzar código sin calidad. La velocidad de entrega no puede ser excusa para degradar la estabilidad del sistema.

**2. Estimaciones Infladas o Irreales**
No tolero que se inflen estimaciones por comodidad (colchon), ni que se oculten impedimentos. Promuevo una cultura de estimaciones honestas basadas en evidencia y comunicación inmediata cuando una fecha corre riesgo (me gusta el poker planning).

**3. Decisiones Técnicas tomadas por Áreas Usuarias**
Las áreas de negocio son dueñas del QUÉ (el requerimiento funcional y la prioridad), pero el equipo de tecnologia (o ingenieria) es el único dueño y responsable del CÓMO (la arquitectura, los patrones de diseño, las bases de datos, las herramientas... etc). No permito que áreas usuarias impongan decisiones arquitectónicas (hoy en dia es muy comun por la IA; es facil cuestionar estas decisiones incluso desde el desconocimiento).

---

## 6. ¿Cómo manejas una situación en la que te contactan constantemente por diferentes canales (correo, chat, reuniones, llamadas) y todos esperan una respuesta rápida?

### Mi sistema de gestión de interrupciones:

**1. Priorización por Impacto y Valor de Negocio**
Filtro y priorizo la atención de mis mensajes alineado estrictamente con la estrategia de la compañía. Atiendo de forma inmediata aquello que genera mayor valor o que impacta directamente a los proyectos críticos de alta prioridad (ej. caídas de producción).

**2. Delegación Activa en el Equipo**
Para todo lo demás que no requiera mi intervención directa, busco sistemáticamente en quién del equipo se puede delegar. Si preguntan sobre un endpoint que desarrolló un dev, lo delego y empodero a él para que responda. Así evito ser el cuello de botella del equipo y fomento el liderazgo y la autonomía de los demás desarrolladores.

**3. Protocolo de Comunicación Asíncrona**
Establezco ventanas de tiempo para revisar canales secundarios (como correos o chats generales de consulta), educando a los demas a que en un momento del dia recibiran atencion.
