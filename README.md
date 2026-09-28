# Sistema de Control de Asistencia
### Proyecto académico — Integración de Competencias II — Ingeniería en Informática

> **Estado del proyecto:** versión final compilada, ejecutada y probada por el equipo en Windows
> con Visual Studio y SQL Server Management Studio. La ejecución final de las pruebas automatizadas
> (27-09-2026) dio **41 pruebas ejecutadas, 41 superadas, 0 con errores y 0 omitidas**. El detalle
> del desarrollo, las pruebas y las evidencias está en el Informe Final y en el Plan de Pruebas.

---

## 1. Tecnología seleccionada

| Decisión | Elección | Motivo |
|---|---|---|
| Lenguaje | C# | Exigido por el enunciado |
| Plataforma | .NET 8 | Versión LTS actual, estable, con soporte oficial de Microsoft.Data.SqlClient |
| Interfaz de escritorio | **WPF** (no .NET MAUI) | MAUI está pensado para apps multiplataforma (incluyendo móvil); para un MVP de escritorio **solo Windows**, WPF es más simple, tiene mejor soporte de `DataGrid`/formularios de negocio, y es la tecnología de escritorio más estable y documentada para este tipo de proyecto académico |
| Motor de base de datos | Microsoft SQL Server | Motor relacional adecuado a la relación 1:N entre Usuario y Asistencia |
| Administración de BD | SQL Server Management Studio 22 | Herramienta usada para crear y verificar la base de datos |
| Acceso a datos | ADO.NET + `Microsoft.Data.SqlClient`, consultas parametrizadas | Librería oficial y actual de Microsoft para SQL Server desde .NET; evita inyección SQL |
| Hashing de contraseñas | PBKDF2-HMACSHA256 vía `Rfc2898DeriveBytes` (nativo de .NET) | Seguro y **no requiere ningún paquete NuGet adicional** |
| Pruebas | xUnit | Estándar moderno, ligero, bien soportado por Visual Studio |

Esta elección se mantuvo sin cambios durante todo el proyecto.

---

## 2. Arquitectura

Arquitectura en capas, simple y sin sobre-ingeniería, apropiada para un MVP académico:

```
SistemaControlAsistencia/
│
├── SistemaControlAsistencia.sln
│
├── src/SistemaControlAsistencia.App/         (proyecto WPF ejecutable)
│   ├── App.xaml / App.xaml.cs                (estilos globales + arranque)
│   ├── appsettings.json                      (cadena de conexión a SQL Server)
│   │
│   ├── Models/                               ← AVANCE #1
│   │   ├── Usuario.cs
│   │   ├── Asistencia.cs
│   │   └── TipoRegistro.cs                   (enum Entrada/Salida)
│   │
│   ├── Data/                                 ← Acceso a datos (AVANCE #2)
│   │   ├── ConfiguracionApp.cs                (lee appsettings.json)
│   │   ├── DatabaseHelper.cs                  (fábrica de SqlConnection)
│   │   ├── IUsuarioRepository.cs / UsuarioRepository.cs
│   │   └── IAsistenciaRepository.cs / AsistenciaRepository.cs
│   │
│   ├── Services/                             ← Lógica de negocio
│   │   ├── AutenticacionService.cs            (CA-01 / Login)
│   │   ├── UsuarioService.cs                  (GU-01, GU-02, GU-03)
│   │   ├── AsistenciaService.cs               (CA-01 marcar entrada/salida)
│   │   ├── ReporteService.cs                  (RE-01, RE-02, RE-03)   ← AVANCE #4
│   │   ├── SesionActual.cs                    (sesión del usuario autenticado)
│   │   ├── ResultadoOperacion.cs              (envoltorio éxito/mensaje)
│   │   └── FabricaServicios.cs                (construcción de servicios para las vistas)
│   │
│   ├── Helpers/
│   │   ├── PasswordHasher.cs                  (PBKDF2)
│   │   ├── ReglasHorarias.cs                  (09:30 y 17:30 centralizados)
│   │   ├── Roles.cs                           (constantes de rol)
│   │   └── Validaciones.cs                    (correo, textos, contraseña)
│   │
│   └── Views/                                 (XAML + code-behind)
│       ├── LoginWindow.xaml(.cs)
│       ├── AdminMainWindow.xaml(.cs)           (menú lateral del administrador)
│       ├── EmpleadoMainWindow.xaml(.cs)        (ventana simplificada del empleado)
│       ├── DashboardView.xaml(.cs)
│       ├── AsistenciaView.xaml(.cs)            (marcar entrada/salida)
│       ├── UsuariosView.xaml(.cs)
│       ├── UsuarioFormWindow.xaml(.cs)         (crear/editar usuario)
│       ├── ReporteAtrasosView.xaml(.cs)
│       ├── ReporteSalidasAnticipadasView.xaml(.cs)
│       └── ReporteInasistenciasView.xaml(.cs)
│
├── tests/SistemaControlAsistencia.Tests/     (proyecto de pruebas xUnit)
│   ├── Fakes/
│   │   ├── FakeUsuarioRepository.cs           (repositorio en memoria)
│   │   └── FakeAsistenciaRepository.cs
│   ├── ReglasHorariasTests.cs                 (pruebas unitarias RE-01/RE-02)
│   ├── PasswordHasherTests.cs
│   ├── AutenticacionServiceTests.cs
│   ├── AsistenciaServiceTests.cs
│   ├── UsuarioServiceTests.cs
│   ├── ReporteServiceTests.cs
│   ├── RolesYSesionTests.cs
│   └── IntegracionBaseDatosTests.cs            ← PRUEBAS DE INTEGRACIÓN (requieren SQL Server real)
│
└── Database/
    ├── BaseDatosAsistencia.txt                 ← AVANCE #2 (script SQL Server completo)
    └── Migracion_EliminarUsuario_Cascade.txt   (solo para bases creadas antes del cambio de GU-03)
```

**Por qué no hay sobre-arquitectura:** no se usan contenedores de inyección de dependencias,
ni CQRS, ni MediatR, ni ORM. `FabricaServicios` construye los servicios a mano; los repositorios
usan ADO.NET directo. Esto es intencional: es un MVP académico de 25 trabajadores, no un sistema
empresarial de gran escala, y así el código se mantiene simple y fácil de mantener.

### Por qué las clases de prueba usan repositorios "falsos" (Fakes) en vez de mocks con librerías
Se definieron `IUsuarioRepository` e `IAsistenciaRepository` como interfaces. Los servicios
(`AutenticacionService`, `UsuarioService`, `AsistenciaService`, `ReporteService`) reciben la
interfaz por constructor, nunca la implementación concreta. Esto permite:
- En la aplicación real: inyectar `UsuarioRepository`/`AsistenciaRepository` (SQL Server real).
- En las pruebas unitarias: inyectar `FakeUsuarioRepository`/`FakeAsistenciaRepository` (listas en
  memoria), sin necesitar Moq ni ninguna librería de mocking adicional, y sin depender de que haya
  una base de datos disponible para poder ejecutar `dotnet test`.

---

## 3. Modelo relacional

```
┌─────────────────────────────┐        ┌──────────────────────────────┐
│           USUARIO           │        │           ASISTENCIA          │
├─────────────────────────────┤        ├──────────────────────────────┤
│ PK IdUsuario        INT     │ 1    N │ PK IdAsistencia     INT       │
│    Nombre           NVARCHAR│───────▶│ FK IdUsuario        INT       │
│    Apellidos        NVARCHAR│        │    TipoRegistro     NVARCHAR  │
│ UQ Correo           NVARCHAR│        │    FechaHora        DATETIME2 │
│    ContrasenaHash   NVARCHAR│        └──────────────────────────────┘
│ CK Rol              NVARCHAR│
│    Estado           BIT     │
└─────────────────────────────┘
```

- **USUARIO 1 : N ASISTENCIA**: un usuario puede tener muchos registros de asistencia; cada
  asistencia pertenece a un único usuario (`FK_Asistencia_Usuario`).
- `UQ_Usuario_Correo`: evita correos duplicados directamente a nivel de base de datos, como
  respaldo de la validación que ya hace `UsuarioService`.
- `CK_Usuario_Rol` y `CK_Asistencia_TipoRegistro`: evitan que se inserten valores fuera de
  `Administrador/Empleado` o `Entrada/Salida`, incluso si alguien escribe directamente en SQL.
- No se agregó ninguna tabla adicional (por ejemplo, una tabla de "Roles" separada): con solo dos
  roles fijos, un `CHECK` es suficiente y evita una tabla y un JOIN innecesarios para un MVP.

### Por qué GU-03 usa `DELETE` físico con `ON DELETE CASCADE` (no eliminación lógica)
La tabla `ASISTENCIA` tiene una `FOREIGN KEY` hacia `USUARIO`. Para que GU-03 elimine realmente al
usuario de la base de datos —tal como lo pide de forma literal el enunciado académico ("el sistema
elimina al usuario seleccionado")—, la restricción `FK_Asistencia_Usuario` se definió con
`ON DELETE CASCADE`: al eliminar un usuario, SQL Server elimina automáticamente sus registros de
asistencia asociados, en la misma operación, sin dejar registros huérfanos ni rechazar el `DELETE`.

Por eso `UsuarioService.EliminarUsuario` llama a `IUsuarioRepository.Eliminar`, que ejecuta un
`DELETE FROM Usuario` directo (la cascada hacia `ASISTENCIA` la resuelve el propio motor de base de
datos). Esta operación es **irreversible** y borra también el historial de asistencia del
trabajador; por eso la interfaz (`UsuariosView`) pide una doble confirmación antes de ejecutarla:
un primer aviso general y un segundo aviso explícito advirtiendo que se perderá el historial de
forma permanente.

Nota de diseño: el campo `Estado` (Activo/Inactivo) del usuario sigue existiendo de forma
independiente y se puede seguir usando desde "Editar" para desactivar temporalmente el acceso de un
trabajador sin perder sus datos ni su historial; "Eliminar" es ahora una acción distinta y separada,
pensada para un borrado definitivo.

---

## 4. Flujo del sistema (resumen)

1. `LoginWindow` pide correo/contraseña → `AutenticacionService.Login` consulta `USUARIO` real.
2. Según `Rol`, se abre `AdminMainWindow` (menú completo) o `EmpleadoMainWindow` (solo asistencia).
3. Ambos roles pueden marcar Entrada/Salida en `AsistenciaView`, validado por `AsistenciaService`
   contra los registros de **hoy** del usuario autenticado (`SesionActual`).
4. El administrador, además, puede:
   - Ver `DashboardView` (indicadores calculados en vivo desde la base de datos).
   - Administrar usuarios en `UsuariosView` + `UsuarioFormWindow` (crear/editar/"eliminar").
   - Ver los tres reportes (`ReporteAtrasosView`, `ReporteSalidasAnticipadasView`,
     `ReporteInasistenciasView`), cada uno con selector de fecha.
5. `Cerrar sesión` llama a `SesionActual.CerrarSesion()` y vuelve a `LoginWindow`, cerrando la
   ventana protegida anterior para que no pueda seguir usándose.

---

## 5. Verificación de rol (no solo se esconden botones)

- El menú lateral de `AdminMainWindow` solo aparece si el usuario autenticado es Administrador.
- **Además**, el constructor de `AdminMainWindow` verifica `SesionActual.EsAdministrador`; si
  alguna vez se intentara abrir esa ventana sin ese rol, se cierra de inmediato y vuelve al Login
  (ver prueba `RolesYSesionTests`).
- `EmpleadoMainWindow` ni siquiera contiene los botones de Usuarios/Reportes/Dashboard: no existen
  en esa ventana, por lo que no hay nada que "esconder".

---

## 6. Credenciales de demostración

| Rol | Correo | Contraseña |
|---|---|---|
| **Administrador** | `nicol.castro@empresa.cl` | `Admin123!` |
| Empleado (activo, puntual) | `ana.torres@empresa.cl` | `Empleado123!` |
| Empleado (activo, límite exacto 09:30/17:30) | `pedro.soto@empresa.cl` | `Empleado123!` |
| Empleado (activo, atraso) | `carlos.fuentes@empresa.cl` | `Empleado123!` |
| Empleado (activo, sin registros el 2026-08-24) | `maria.lopez@empresa.cl` | `Empleado123!` |
| Empleado (**inactivo** — para probar el mensaje de bloqueo) | `jorge.diaz@empresa.cl` | `Empleado123!` |
| Empleado (activo, salida anticipada) | `laura.reyes@empresa.cl` | `Empleado123!` |

Estas contraseñas son exclusivamente de demostración académica; no son contraseñas reales de
ninguna persona.

---

## 7. Instrucciones de ejecución desde cero

### 7.1 Requisitos previos
- Windows 10/11.
- **Visual Studio 2022** (versión 17.9 o superior) con el workload **".NET desktop development"**
  instalado (incluye soporte para WPF).
- **SQL Server** (Express, Developer o LocalDB) y **SQL Server Management Studio 22 (SSMS)**.

### 7.2 Configurar la base de datos
1. Abra **SQL Server Management Studio 22**.
2. Conéctese a su instancia (por ejemplo `localhost\SQLEXPRESS`) con Windows Authentication.
3. Haga clic en **Nueva consulta**.
4. Abra `Database/BaseDatosAsistencia.txt`, copie todo su contenido y péguelo en la consulta.
5. Presione **Ejecutar (F5)**. Se creará la base `ControlAsistencia` con tablas, restricciones,
   la cuenta administradora y los usuarios/asistencias de prueba.
6. En el **Object Explorer**, expanda `ControlAsistencia > Tablas` para confirmar que `Usuario` y
   `Asistencia` existen; haga clic derecho → "Seleccionar las 1000 filas superiores" para ver los
   datos cargados.
7. Para obtener la cadena de conexión que usará C#: haga clic derecho sobre el servidor en el
   Object Explorer → **Propiedades** → copie el nombre del servidor (ej. `localhost\SQLEXPRESS`).
   Si usa autenticación de Windows (recomendado para este MVP), la cadena de conexión ya
   preconfigurada en `appsettings.json` normalmente funciona sin cambios.

### 7.3 Abrir el proyecto en Visual Studio
1. Abra Visual Studio 2022.
2. **Abrir un proyecto o solución** → seleccione `SistemaControlAsistencia.sln`.
3. Espere a que Visual Studio restaure los paquetes NuGet automáticamente
   (`Microsoft.Data.SqlClient`, `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`).
   Si no restaura solo, haga clic derecho sobre la solución → **Restaurar paquetes NuGet**.

### 7.4 Configurar la cadena de conexión
1. Abra `src/SistemaControlAsistencia.App/appsettings.json`.
2. Si su instancia de SQL Server no se llama `localhost\SQLEXPRESS`, reemplace el valor de
   `Server=` por el nombre de su instancia. Ejemplo para LocalDB:
   ```json
   "ControlAsistencia": "Server=(localdb)\\MSSQLLocalDB;Database=ControlAsistencia;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
3. Guarde el archivo.

### 7.5 Compilar y ejecutar
1. En el **Explorador de soluciones**, haga clic derecho sobre `SistemaControlAsistencia.App` →
   **Establecer como proyecto de inicio**.
2. Presione **F5** (o el botón ▶ "SistemaControlAsistencia.App").
3. Debería abrirse la ventana de **Login**.
4. Inicie sesión con `nicol.castro@empresa.cl` / `Admin123!`.

### 7.6 Probar cada requerimiento
- **Crear un empleado (GU-01):** en el menú lateral → **Usuarios** → **+ Nuevo usuario**, complete
  el formulario y guarde.
- **Probar entrada/salida (CA-01):** cierre sesión e inicie con `ana.torres@empresa.cl` /
  `Empleado123!`, presione **Marcar Entrada**; observe que el botón se deshabilita y **Marcar
  Salida** se habilita. Presione **Marcar Salida**.
- **Probar atrasos (RE-01):** inicie sesión como administrador → **Reportes → Atrasos** →
  seleccione la fecha **24-08-2026**; debería aparecer **Carlos Fuentes** (entrada 09:45).
- **Probar salidas anticipadas (RE-02):** **Reportes → Salidas anticipadas** → fecha
  **24-08-2026**; debería aparecer **Laura Reyes** (salida 17:15).
- **Probar inasistencias (RE-03):** **Reportes → Inasistencias** → fecha **24-08-2026**; debería
  aparecer **Maria Lopez** (no tiene ningún registro ese día).

### 7.7 Ejecutar las pruebas unitarias e integración
1. Menú **Prueba → Explorador de pruebas** (Test Explorer).
2. Haga clic en **Ejecutar todas las pruebas**.
3. Las pruebas de `ReglasHorariasTests`, `PasswordHasherTests`, `AutenticacionServiceTests`,
   `AsistenciaServiceTests`, `UsuarioServiceTests`, `ReporteServiceTests` y `RolesYSesionTests`
   **no requieren SQL Server** (usan repositorios falsos en memoria) y deberían pasar siempre.
4. Las pruebas de `IntegracionBaseDatosTests` **sí requieren** que ya haya ejecutado el script de
   la sección 7.2 y que `appsettings.json` del proyecto de pruebas apunte a su instancia real de
   SQL Server; si SQL Server no está disponible, fallarán con un mensaje explícito indicando que
   falta levantar la base de datos (esto es intencional, no un error del código).

---

## 8. Manejo de errores implementado

Todas las vistas envuelven sus llamadas a los servicios en `try/catch` y muestran un
`MessageBox` con un mensaje comprensible en vez de dejar que la aplicación se cierre. Casos
cubiertos: base de datos no disponible (`LoginWindow`, todas las vistas), credenciales
incorrectas, campos vacíos, correo duplicado, usuario inexistente/inactivo, registro de
asistencia duplicado, operación no autorizada (verificación de rol en `AdminMainWindow`), error
al guardar/consultar.

---

## 9. Matriz de trazabilidad

| Requerimiento | Clase/Servicio | Tabla | Método | Pantalla | Prueba |
|---|---|---|---|---|---|
| CA-01 (login) | `AutenticacionService` | USUARIO | `Login` | `LoginWindow` | `AutenticacionServiceTests` |
| CA-01 (marcar asistencia) | `AsistenciaService` | ASISTENCIA | `RegistrarEntrada` / `RegistrarSalida` | `AsistenciaView` | `AsistenciaServiceTests` |
| GU-01 | `UsuarioService` | USUARIO | `CrearUsuario` | `UsuariosView` + `UsuarioFormWindow` | `UsuarioServiceTests` |
| GU-02 | `UsuarioService` | USUARIO | `ModificarUsuario` | `UsuariosView` + `UsuarioFormWindow` | `UsuarioServiceTests` |
| GU-03 | `UsuarioService` | USUARIO + ASISTENCIA | `EliminarUsuario` (DELETE físico con cascada) | `UsuariosView` | `UsuarioServiceTests` |
| RE-01 | `ReporteService` | USUARIO + ASISTENCIA | `ObtenerAtrasos` | `ReporteAtrasosView` | `ReporteServiceTests`, `ReglasHorariasTests` |
| RE-02 | `ReporteService` | USUARIO + ASISTENCIA | `ObtenerSalidasAnticipadas` | `ReporteSalidasAnticipadasView` | `ReporteServiceTests`, `ReglasHorariasTests` |
| RE-03 | `ReporteService` | USUARIO + ASISTENCIA | `ObtenerInasistencias` | `ReporteInasistenciasView` | `ReporteServiceTests` |
| Roles (control de acceso) | `SesionActual` | USUARIO | `EsAdministrador` | `AdminMainWindow` | `RolesYSesionTests` |
| Integridad completa (flujo real) | Todos los anteriores | USUARIO + ASISTENCIA | — | — | `IntegracionBaseDatosTests` |

---

## 10. Pruebas automatizadas: resultado final

El proyecto `SistemaControlAsistencia.Tests` contiene **41 pruebas xUnit**: 38 unitarias (con
repositorios Fake en memoria) y 3 de integración (contra SQL Server real). El detalle de cada caso
está en el documento **Plan de Pruebas Unitarias** entregado junto al proyecto.

| Clase de pruebas | Tipo | Pruebas | Resultado |
|---|---|---|---|
| `AsistenciaServiceTests` | Unitaria | 7 | Superadas |
| `AutenticacionServiceTests` | Unitaria | 5 | Superadas |
| `UsuarioServiceTests` | Unitaria | 8 | Superadas |
| `ReglasHorariasTests` | Unitaria (Theory, 8 casos) | 8 | Superadas |
| `ReporteServiceTests` | Unitaria | 4 | Superadas |
| `RolesYSesionTests` | Unitaria | 3 | Superadas |
| `PasswordHasherTests` | Unitaria | 3 | Superadas |
| `IntegracionBaseDatosTests` | Integración | 3 | Superadas |
| **Total** | | **41** | **41 superadas, 0 con errores, 0 omitidas** |

Durante la ejecución se detectaron y corrigieron tres problemas (plataforma de destino del
proyecto de pruebas, un dato de prueba en la eliminación física y una prueba de integración que no
se podía repetir); están documentados en el Informe Final.

---

## 11. Avance #1 (Semana 2) — archivos correspondientes

- `src/SistemaControlAsistencia.App/Models/Usuario.cs`
- `src/SistemaControlAsistencia.App/Models/Asistencia.cs`
- `src/SistemaControlAsistencia.App/Models/TipoRegistro.cs`
- Estructura inicial de carpetas: `Models/`, `Data/`, `Services/`, `Helpers/`, `Views/`

Para entregar solo el Avance #1: comprima estos archivos manteniendo la misma estructura de
carpetas relativa (`Models/Usuario.cs`, etc.).

---

## 12. Avance #2 (Semana 3) — archivos correspondientes

- `Database/BaseDatosAsistencia.txt` (script completo de creación)
- Sección 3 de este README (modelo relacional y explicación de relaciones)
- `src/SistemaControlAsistencia.App/Data/*` (repositorios que consumen ese modelo)

---

## 13. Avance #3 (Semana 4) — archivos correspondientes

- `src/SistemaControlAsistencia.App/Services/UsuarioService.cs` y `AsistenciaService.cs`
- `src/SistemaControlAsistencia.App/Views/UsuariosView.xaml(.cs)`, `UsuarioFormWindow.xaml(.cs)` y `AsistenciaView.xaml(.cs)`
- `tests/SistemaControlAsistencia.Tests/UsuarioServiceTests.cs` y `AsistenciaServiceTests.cs`

---

## 14. Avance #4 (Semana 5) — archivos correspondientes

- `src/SistemaControlAsistencia.App/Services/ReporteService.cs`
- `src/SistemaControlAsistencia.App/Views/ReporteAtrasosView.xaml(.cs)`
- `src/SistemaControlAsistencia.App/Views/ReporteSalidasAnticipadasView.xaml(.cs)`
- `src/SistemaControlAsistencia.App/Views/ReporteInasistenciasView.xaml(.cs)`
- `tests/SistemaControlAsistencia.Tests/ReporteServiceTests.cs`
- `tests/SistemaControlAsistencia.Tests/IntegracionBaseDatosTests.cs`

---

## 15. Preguntas de cierre

**1. ¿Qué aprendiste de la actividad realizada?**
Aprendí a estructurar una aplicación de escritorio en capas (modelos, acceso a datos, servicios,
vistas) de manera que la lógica de negocio —como las reglas de atraso a las 09:30 o la validación
de doble entrada— quede separada de la interfaz gráfica y de la base de datos. Esto permitió
escribir pruebas unitarias reales para `ReglasHorarias` y los servicios sin necesitar una
conexión a SQL Server, usando repositorios falsos en memoria que implementan las mismas
interfaces que los repositorios reales.

**2. ¿En qué ámbitos puedes utilizar o aplicar lo realizado en la actividad?**
El mismo patrón (interfaz de repositorio + servicio + vista) es aplicable a cualquier sistema de
gestión con base de datos relacional: inventario, matrícula de alumnos, control de citas médicas,
etc. La técnica de centralizar reglas de negocio (como las horas límite) en una sola clase también
es reutilizable en cualquier sistema con reglas que puedan cambiar con el tiempo.

**3. ¿Hubo algún término, definición o parte del proceso que necesites reforzar? ¿Cuál?**
Reforzar las consultas con fechas y JOIN en T-SQL, los conceptos de claves foráneas (por ejemplo
ON DELETE CASCADE, que usamos por primera vez) y el diseño de pruebas de integración repetibles,
tema que aprendimos al corregir la falla de la prueba de asistencia. Las respuestas de cada avance
están en la sección 11 del Informe Final.
