# Ayuda de ChangeTracker

Guía local de la versión de desarrollo. La app observa configuraciones sin cambiarlas. No repara Windows ni decide si el equipo es seguro.

## Idioma

Elija **Configuración > Idioma**. Se recuerda la elección y cambian interfaz, ayuda, fechas e informes de texto sin reiniciar ni capturar. Las veinte traducciones están incluidas sin conexión. Árabe, árabe egipcio y urdu usan contenido de derecha a izquierda; el menú sigue a la izquierda.

Los nombres, rutas, identificadores y valores reales no se traducen. JSON/CSV mantienen campos en inglés para compatibilidad. UAC usa el idioma de Windows. Se necesita revisión lingüística nativa antes de publicar.

## Configuración

Abra Configuración en el menú. Las preferencias se guardan para la carpeta de historial actual y se recuperan al reiniciar.

- Apariencia incluye tema claro/oscuro, fuente y colores independientes para texto de la app, etiquetas, fondo y texto de botones. Las muestras con nombre ofrecen Predeterminado, Azul marino, Verde bosque, Granate y Morado. Predeterminado restaura el color del tema; el alto contraste de Windows tiene prioridad y las acciones principales mantienen texto contrastante.
- Las capturas automáticas están desactivadas inicialmente. Intervalos: 15 minutos, 1 hora, 6 horas, diario o semanal. Funcionan solo con la app abierta, también en la bandeja, con acceso normal y cancelación; no piden administrador ni despiertan el equipo. Se comprueba cada minuto si hay trabajo pendiente; al reabrir puede ejecutarse una comprobación vencida, sin repetir todos los intervalos perdidos.
- Retención conserva todo por defecto. Con 30, 90, 180 o 365 días, elimina solo capturas antiguas sin nombre que no sean referencias. La limpieza se ejecuta cuando corresponde por primera vez, después diariamente mientras la app está abierta y tras capturas automáticas correctas; también funciona con las capturas automáticas desactivadas. Puntos con nombre y referencias de todos los ámbitos quedan protegidos.
- Iniciar al entrar en Windows es opcional y está desactivado. Incluye el inicio de sesión después de reiniciar, no la captura antes de entrar. Solo cambia la entrada de inicio de esta app para este usuario; no instala servicio ni tarea de arranque, ni cambia otras apps o políticas. Un fallo conserva la elección anterior.
- Seguir en la bandeja también es opcional y está desactivado. Minimizar o Cerrar oculta la ventana y continúa las comprobaciones. Abrir o iniciar otra copia la restaura. Salir desde la bandeja cancela el trabajo y termina. Sin esta opción, Cerrar cancela y termina.

Ambos ámbitos están seleccionados en perfiles nuevos; las elecciones guardadas se respetan. Todo el historial retenido puede consultarse desde cualquier ámbito, pero los extremos de una comparación deben tener ámbito y acceso compatibles. Ninguna preferencia eleva permisos. El perfilado de recursos y la validación del ciclo de instalación siguen pendientes.

## Primeros pasos

1. Abra normalmente, no como administrador.
2. Revise las casillas **Usuario actual** y **Todo el equipo**, ambas activadas en un perfil nuevo. Mantenga una o ambas, nunca ninguna; confirme.
3. Revise **Fuentes**. Red y PATH son opcionales. Elegir no inicia una comprobación.
4. Seleccione **Hoy (nueva captura)** y **Comprobar ahora**.

La primera observación utilizable establece una referencia para su ámbito y acceso. Es inventario, no reconstrucción de cambios pasados. En perfiles nuevos las capturas automáticas están desactivadas; los intervalos habilitados solo funcionan mientras la app está abierta.

## Simple y Avanzado

Simple muestra resúmenes y texto. Avanzado añade **Todos los campos registrados**, valores anteriores/posteriores sin límite de resumen y metadatos técnicos, además de JSON/CSV. Ambos permiten comparar fechas, administrar historial y elegir fuentes.

Cambiar de modo no captura, eleva permisos, mueve referencias ni cobra más. Precio previsto: 0,99 USD una vez, ambos modos incluidos; la vista previa no tiene compra.

## Ámbitos independientes

Usuario actual lee sus registros de aplicaciones, Run/RunOnce, asociaciones, audio, proxy y PATH. Todo el equipo lee registros compartidos, servicios, tareas, actualizaciones, controladores, firewall, DNS/DHCP y PATH del sistema. No carga perfiles privados ajenos.

Con ambas casillas se leen las partes por separado y se guarda una observación combinada. Solo la parte del equipo puede elevarse, mediante petición explícita; el usuario conserva su cuenta normal. Se respetan selecciones anteriores y preferencias de fuentes por ámbito.

## Fechas y observaciones

Los selectores ofrecen solo capturas retenidas con fecha local, hora con milisegundos, desfase UTC, punto y ámbito/acceso. No admiten fechas escritas libremente; las capturas borradas desaparecen. El segundo extremo puede ser otra captura guardada o una nueva comprobación de Hoy. **Referencia** selecciona la referencia normal, sin reemplazarla. **Solo estado actual** borra la selección anterior.

Un día sin instantánea no tiene datos comparables. No se inventan datos ni se usa silenciosamente el día más cercano. Las observaciones deben ser distintas, cronológicas, no superpuestas y con ámbito/acceso coincidentes.

## Dos instantáneas guardadas

Elija fecha y captura anteriores. Seleccione **Instantánea guardada**, la fecha posterior y la captura exacta; pulse **Comparar instantáneas**. No se ejecuta un colector, no se pide UAC ni se guarda otro registro. La referencia no cambia. Se pueden comparar dos horas del mismo día.

## Una instantánea y hoy

Elija la referencia deseada y **Hoy (nueva captura)**. **Comprobar ahora** captura y compara con esa selección, no con otra referencia oculta. El panel se contrae tras éxito y puede reabrirse. Una selección pendiente no altera el informe mostrado.

Si la referencia usó administrador, solicite esa acción explícitamente o elija solo estado actual. Cancelar detiene la captura y conserva el historial. Con la bandeja activada, Cerrar mantiene la captura; Salir desde la bandeja la cancela y termina la app.

## Permisos de administrador

Muchos datos del equipo se leen sin elevar. Fuentes inaccesibles quedan como lagunas. **Comprobar como administrador** requiere clic, confirmación predeterminada en No y autorización UAC para una sola comprobación.

La ventana queda normal. Un ayudante temporal de solo lectura comprueba el equipo; no instala servicio ni conserva permiso. Denegar no modifica referencia o historial. No comparta contraseñas ni desactive protecciones. Cancelar en la app no cierra un UAC ya abierto: responda en Windows.

## Detalles de cambios

Añadido, Eliminado y Modificado describen extremos observados, no el autor, causa ni instante exacto. Importante/Revisar indican prioridad, no malware. Las actividades habituales/esperadas y los cambios sin impacto evaluado tienen grupos separados. El título cuenta los hallazgos del filtro; **Ver todos** muestra más de tres.

Avanzado conserva contexto sin cambios, campos nuevos/eliminados, valores largos, identidad y origen. Metadatos incluyen IDs, ámbito/acceso, tiempos UTC de captura y lectura, versiones, estado y recuentos. Vacío y ausente son distintos. Comandos nunca guardados no pueden reconstruirse; claves y huellas siguen ocultas. Identificadores pueden reconocer el dispositivo: revise antes de copiar.

**Marcar como esperado** solo anota esa ocurrencia y se puede deshacer. **Abrir configuración** abre un destino permitido; no repara nada.

## Cobertura e incertidumbre

Correcto significa completo dentro del subconjunto implementado. Parcial significa entradas inaccesibles o límites; Error, lectura inútil; Desactivado/fuera del ámbito, no leído. Nunca se infieren eliminaciones de datos incompletos.

La lectura actual puede estar completa y aun así no compararse con una anterior incompleta o incompatible en formato/clave. Con ambos ámbitos, una parte incompleta hace parcial la categoría combinada. **Detalles de cobertura** incluye referencia y áreas sin cambios. No es un veredicto general de seguridad.

## Referencias e historial

En **Instantáneas** puede ver, nombrar (1–120 caracteres), borrar o cambiar la referencia con confirmación. Debe sustituir una referencia antes de borrarla. Usuario, equipo, ambos, niveles de acceso y registros antiguos mixtos tienen referencias separadas.

No hay límite fijo de puntos. La retención opcional limpia capturas antiguas sin nombre; protege puntos con nombre y todas las referencias. Un resultado totalmente inútil no se guarda. Borrar historial elimina capturas y marcas tras confirmación, pero conserva preferencias y clave. No borra exportaciones ni Windows; no es borrado forense.

## Fuentes y límites

| Fuente | Límite |
| --- | --- |
| Apps e inicio | Registros de desinstalación y Run/RunOnce; no Store, portables ni carpeta Inicio. Registro no prueba ejecución. |
| Servicios y tareas | Configuración accesible; sin ejecución, sondeo continuo ni comandos/XML guardados. |
| Actualizaciones y drivers | Historial local exitoso con máximo 5.000 eventos (si excede, parcial), metadatos WMI; sin instalación, firmware ni reversión. |
| Predeterminadas y audio | Asociaciones compatibles y dispositivos predeterminados; sin grabación ni cambios. |
| Protección | Perfiles de firewall, no evaluación antivirus. |
| Red y PATH | Opcionales: proxy o DNS/DHCP y PATH persistente; sin paquetes, contraseñas, exploración o variables adicionales. |

Tiempo límite por fuente: 25 segundos. Inventario visible: 1.000 registros por fuente; se conserva todo lo realmente capturado. Solo lectura permite escribir el historial propio y exports solicitados, no ajustes monitorizados.

## Informes

**Informe** describe el resultado mostrado, no selecciones pendientes. Previsualice, copie o guarde texto en ambos modos; Avanzado añade JSON/CSV. El texto se localiza, los formatos estructurados conservan el esquema inglés. Nada se envía automáticamente.

Se omiten claves, huellas, comandos, nombres de puntos e IDs de audio en reportes. Se ocultan rutas y patrones de credenciales; CSV neutraliza fórmulas. Pueden quedar nombres identificativos. No hay PDF/HTML, importación ni paquetes cifrados. Los exports permanecen tras borrar el historial.

## Privacidad y almacenamiento

Ruta normal: `%LOCALAPPDATA%\PCChangeTracker`; Ajustes muestra la real. SQLite no está cifrada. La clave usa DPAPI del usuario actual: copiarla a otra cuenta no garantiza descifrarla. Respalde de forma segura antes de versiones nuevas.

El formato de historial 2 conserva los registros antiguos como mixtos y bloquea lectores antiguos. El ayudante recibe categorías y una clave temporal, nunca rutas de historial ni comandos arbitrarios; solo la ventana normal guarda. El ciclo de datos de MSIX necesita pruebas aparte.

## Accesibilidad

Tab/Mayús+Tab, flechas y espacio permiten navegar. Ámbito usa casillas, modos usan radio. Prioridad y tipo tienen texto además de color. Hay foco visible y colores Windows de alto contraste.

F1 abre Ayuda, Ctrl+F busca, Escape cierra. Zoom de ayuda hasta 160%; tablas estrechas pasan a entradas etiquetadas. La validación completa de lectores de pantalla y revisión nativa sigue pendiente.

Los títulos exponen niveles para navegar con lectores de pantalla. Abrir detalles mueve el foco al panel; Tab recorre sus controles y Escape lo cierra. Configuración permite elegir fuente y colores adaptados al tema; alto contraste tiene prioridad.

## Problemas habituales

Fecha vacía: elija otra observación. Comparación rechazada: compruebe cronología, ámbito y acceso. Fuente parcial: no significa eliminación. Informe antiguo: ejecute la nueva selección. Base inaccesible: revise espacio, permisos y versión antes de borrar nada.

Para soporte use un informe revisado y versiones de app/Windows, nunca contraseña, base bruta ni clave. Denegar administrador no impide la comprobación ordinaria.

## Estado de publicación

Vista previa con 11 categorías acotadas, comprobaciones manuales o programadas opcionales y retención configurable. No hay monitor continuo de eventos, notificaciones ni línea temporal completa. La colección consume recursos; no se promete CPU cero.

MSI/MSIX x64 locales sin firma; MSI necesita aprobación para instalar, aparte del consentimiento de captura. No instale ambos juntos. UAC real, cuenta administradora alternativa, Windows 10/ARM64, instalación y aprobación Store de `allowElevation` siguen pendientes. Cambiar código no reconstruye paquetes previos. Las imágenes Store no sustituyen capturas reales ni certificación.