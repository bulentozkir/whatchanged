# Ayuda de ChangeTracker

Guía local de la versión de desarrollo. La app observa configuraciones sin cambiarlas. No repara Windows ni decide si el equipo es seguro.

## Idioma

Elija **Configuración > Idioma**. Se recuerda la elección y cambian interfaz, ayuda, fechas e informes de texto sin reiniciar ni capturar. Las veinte traducciones están incluidas sin conexión. Árabe, árabe egipcio y urdu usan contenido de derecha a izquierda; el menú sigue a la izquierda.

Los nombres, rutas, identificadores y valores reales no se traducen. JSON/CSV mantienen campos en inglés para compatibilidad. UAC usa el idioma de Windows. Se necesita revisión lingüística nativa antes de publicar.

## Configuración

Abra Configuración en el menú. Las preferencias se guardan para la carpeta de historial actual y se recuperan al reiniciar.

- Apariencia incluye tema claro/oscuro, fuente y colores independientes para texto de la app, etiquetas, fondo y texto de botones. Las muestras con nombre ofrecen Predeterminado, Azul marino, Verde bosque, Granate y Morado. Predeterminado restaura el color del tema; el alto contraste de Windows tiene prioridad y las acciones principales mantienen texto contrastante.
- Las capturas automáticas son cada 4 horas por defecto; se conservan las elecciones guardadas, incluido Desactivado. Intervalos: 15 minutos, 1 hora, 4 horas, 6 horas, diario o semanal. Elija Desactivado para comprobar solo manualmente. Funcionan solo con la app abierta, también en la bandeja, con acceso normal y cancelación. Tras confirmar el ámbito, la primera comprobación o una vencida puede ejecutarse en la siguiente revisión por minuto; las siguientes respetan el intervalo. No piden administrador, despiertan el equipo ni repiten todos los intervalos perdidos.
- La retención predeterminada es de 30 días; se conservan las elecciones guardadas, incluido conservar para siempre. Elija 30, 90, 180 o 365 días, o conservar para siempre. Solo se eliminan capturas antiguas sin nombre que no sean referencias. La limpieza se ejecuta cuando corresponde por primera vez, después diariamente mientras la app está abierta y tras capturas automáticas correctas; también funciona con las capturas automáticas desactivadas. Puntos con nombre y referencias de todos los ámbitos quedan protegidos.
- Iniciar al entrar en Windows es opcional y está desactivado. Incluye el inicio de sesión después de reiniciar, no la captura antes de entrar. Solo cambia la entrada de inicio de esta app para este usuario; no instala servicio ni tarea de arranque, ni cambia otras apps o políticas. Un fallo conserva la elección anterior.
- Minimizar o Cerrar siempre oculta la ventana en la bandeja y continúa las comprobaciones. Abrir, hacer doble clic en el icono o iniciar otra copia la restaura. Salir desde la bandeja cancela el trabajo y termina la app. El inicio normal abre maximizado; al restaurar desde la bandeja se mantiene el último estado visible. El inicio de sesión opcional comienza oculto, también después de reiniciar Windows; no funciona antes de iniciar sesión ni como servicio.

Simple muestra los campos públicos modificados con etiquetas Antes/Después y valores más grandes, seleccionables y de solo lectura. Al cerrar los detalles, el foco vuelve al botón de origen si sigue disponible. Avanzado separa las fechas conservadas de las horas exactas de captura, con punto de control y ámbito/acceso en líneas distintas. Cambiar de modo conserva la comparación seleccionada.

Ambos ámbitos están seleccionados en perfiles nuevos; las elecciones guardadas se respetan. Todo el historial retenido puede consultarse desde cualquier ámbito, pero los extremos de una comparación deben tener ámbito y acceso compatibles. Ninguna preferencia eleva permisos. El perfilado de recursos y la validación del ciclo de instalación siguen pendientes.

## Primeros pasos

1. Abra normalmente, no como administrador.
2. Revise las casillas **Usuario actual** y **Todo el equipo**, ambas activadas en un perfil nuevo. Mantenga una o ambas, nunca ninguna; confirme.
3. Revise **Fuentes**. Todas las comprobaciones compatibles, incluidas Red y PATH, están activadas por defecto. Se conservan las elecciones guardadas; puede desactivar fuentes. Elegir no inicia una comprobación.
4. Seleccione **Hoy (nueva captura)** y **Comprobar ahora**.

La primera observación utilizable establece una referencia para su ámbito y acceso. Es inventario, no reconstrucción de cambios pasados. Los perfiles nuevos usan un intervalo de 4 horas, solo tras confirmar el ámbito y mientras la app esté abierta. Elija Desactivado para comprobar solo manualmente.

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

Si la referencia usó administrador, solicite esa acción explícitamente o elija solo estado actual. Cancelar detiene la captura y conserva el historial. Cerrar mantiene la captura en la bandeja; Salir desde la bandeja la cancela y termina la app.

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
| Red y PATH | Activadas por defecto: proxy o DNS/DHCP y PATH persistente; sin paquetes, contraseñas, exploración o variables adicionales. |

Todas las fuentes compatibles se activan por defecto si no hay una elección válida guardada. Una fuente que desactivó permanece desactivada tras una actualización; cámbiela en Fuentes. Activarla no inicia una captura ni eleva permisos. Para mostrar diferencias necesita observaciones válidas en ambos extremos; las capturas antiguas no se completan retroactivamente.

Tiempo límite por fuente: 25 segundos. Inventario visible: 1.000 registros por fuente; se conserva todo lo realmente capturado. Solo lectura permite escribir el historial propio y exports solicitados, no ajustes monitorizados.

## Informes

**Informe** describe el resultado mostrado, no selecciones pendientes. Previsualice, copie o guarde texto en ambos modos; Avanzado añade JSON/CSV. El texto se localiza, los formatos estructurados conservan el esquema inglés. Nada se envía automáticamente.

Se omiten claves, huellas, comandos, nombres de puntos e IDs de audio en reportes. Se ocultan rutas y patrones de credenciales; CSV neutraliza fórmulas. Pueden quedar nombres identificativos. No hay PDF/HTML, importación ni paquetes cifrados. Los exports permanecen tras borrar el historial.

## Privacidad y almacenamiento

Ruta normal: `%LOCALAPPDATA%\PCChangeTracker`; Ajustes muestra la real. SQLite no está cifrada. La clave usa DPAPI del usuario actual: copiarla a otra cuenta no garantiza descifrarla. Respalde de forma segura antes de versiones nuevas.

### Gestionar el espacio

1. Consulte el espacio de capturas encima de Ayuda en la barra izquierda. Aparece en todas las páginas y abarca todos los ámbitos del historial actual.
2. Pase el ratón sobre la etiqueta para ver información. También puede enfocarla con Tab; los lectores de pantalla reciben su nombre y ayuda.
3. En Configuración, ajuste la frecuencia de comprobaciones automáticas. Un intervalo mayor genera menos capturas futuras. Desactivado detiene las capturas automáticas, pero no borra historial ni desactiva la limpieza por retención.
4. Una retención más corta elimina capturas antiguas aptas en la siguiente limpieza programada, no inmediatamente al seleccionar. La limpieza funciona al vencer, luego a diario mientras la app está abierta y tras capturas automáticas correctas. Se protegen todas las referencias y puntos con nombre, por lo que no es un límite estricto de espacio.

La cifra suma `history.db`, `history.db-wal` y `history.db-shm` cuando existen. Incluye preferencias, datos auxiliares y espacio reutilizable, no solo capturas; no es el tamaño asignado por bloques que muestra Windows. Excluye exportaciones, instalación y archivo de clave. Las unidades B, KiB, MiB, GiB y TiB usan múltiplos de 1.024 y formato numérico local.

Se actualiza al refrescar el historial tras capturas, borrado o limpieza; no es un monitor continuo. Tamaño no disponible no significa cero. Borrar puede dejar espacio reutilizable sin reducir el archivo; incluso un historial vacío ocupa espacio. No hay compactación automática. No borre la base ni sus archivos temporales mientras la app esté abierta.

El formato de historial 2 conserva los registros antiguos como mixtos y bloquea lectores antiguos. El ayudante recibe categorías y una clave temporal, nunca rutas de historial ni comandos arbitrarios; solo la ventana normal guarda. El ciclo de datos de MSIX necesita pruebas aparte.

## Accesibilidad

En Configuración > Apariencia, Tamaño del texto ofrece 100%, 125%, 150% y 200%, guarda la elección y amplía páginas, controles, Ayuda e Informe. Los pares de campos pasan a una columna cuando falta espacio; la página, la barra lateral y los diálogos se pueden desplazar. La fuente elegida también se usa en la guía; el zoom propio de Ayuda sigue disponible hasta 160% del tamaño base.

La navegación lateral anuncia la página seleccionada y usa las flechas. Ctrl+1 abre Revisar cambios, Ctrl+2 Instantáneas, Ctrl+3 Fuentes y Ctrl+4 Configuración. F6 y Shift+F6 recorren navegación, barra de comandos y título de la página; Tab continúa por sus controles. En Ayuda, F6 recorre búsqueda, temas y documento; Ctrl+F vuelve a la búsqueda.

El selector de ámbito enfoca su primera casilla y, al confirmar, vuelve a Cambiar ámbito. Los paneles modales desactivan toda la barra lateral y los atajos de página; Tab permanece dentro. Escape cierra los detalles y devuelve el foco a la acción original si existe. Ayuda empieza en búsqueda e Informe en la vista previa de solo lectura. Las filas de capturas, temas, grupos y campos tienen nombres legibles; los valores identifican campo y lado Antes/Después, y las fuentes anuncian estado y alcance. Los controles principales tienen una altura mínima de interacción de 44 unidades independientes del dispositivo.

Estas funciones no certifican conformidad universal. Faltan pruebas manuales con lectores de pantalla, temas de contraste, escalas de Windows y personas con discapacidad. Las pruebas de teclado real requieren una sesión desbloqueada y sin interferencias.

Tab/Mayús+Tab, flechas y espacio permiten navegar. Ámbito usa casillas, modos usan radio. Prioridad y tipo tienen texto además de color. Hay foco visible y colores Windows de alto contraste.

F1 abre Ayuda, Ctrl+F busca, Escape cierra. Zoom de ayuda hasta 160%; tablas estrechas pasan a entradas etiquetadas. La validación completa de lectores de pantalla y revisión nativa sigue pendiente.

Los títulos exponen niveles para navegar con lectores de pantalla. Abrir detalles mueve el foco al panel; Tab recorre sus controles y Escape lo cierra. Configuración permite elegir fuente y colores adaptados al tema; alto contraste tiene prioridad.

## Problemas habituales

Fecha vacía: elija otra observación. Comparación rechazada: compruebe cronología, ámbito y acceso. Fuente parcial: no significa eliminación. Informe antiguo: ejecute la nueva selección. Base inaccesible: revise espacio, permisos y versión antes de borrar nada.

Para soporte use un informe revisado y versiones de app/Windows, nunca contraseña, base bruta ni clave. Denegar administrador no impide la comprobación ordinaria.

## Estado de publicación

Vista previa con 11 categorías acotadas, comprobaciones manuales o programadas opcionales y retención configurable. No hay monitor continuo de eventos, notificaciones ni línea temporal completa. La colección consume recursos; no se promete CPU cero.

MSI/MSIX x64 locales sin firma; MSI necesita aprobación para instalar, aparte del consentimiento de captura. No instale ambos juntos. UAC real, cuenta administradora alternativa, Windows 10/ARM64, instalación y aprobación Store de `allowElevation` siguen pendientes. Cambiar código no reconstruye paquetes previos. Las imágenes Store no sustituyen capturas reales ni certificación.