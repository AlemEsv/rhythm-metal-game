# Flujo de Escenas - Metal Rhythm Game

## Descripción General

Este documento explica el sistema de navegación entre escenas del juego y los componentes que lo hacen funcionar.

## Flujo de Navegación

```txt
IntroScene
    ↓ (ESPACIO)
CalibrationScene
    ↓ (ESPACIO - automático)
CalibrationResultsScene
    ↓ (automático)
MainMenuScene
    ↓ (ENTER o ESPACIO)
MainScene
    ├─→ (Botón Options) → OptionsScene
    └─→ (Botón Play) → SampleScene
```

## Scripts de Navegación

### 1. IntroController.cs

**Ubicación**: `Assets/Scripts/IntroController.cs`

**Función**: Controla la pantalla de introducción con video.

**Características**:

- Reproduce un video en loop
- Detecta la tecla ESPACIO para avanzar
- Utiliza `SceneLoader` para transiciones suaves
- Fallback a `SceneManager` si `SceneLoader` no existe

**Configuración**:

- `nextSceneName`: "CalibrationScene" (por defecto)

### 2. LatencyCalibrator.cs

**Ubicación**: `Assets/Scripts/LatencyCalibrator.cs`

**Función**: Maneja la calibración del ritmo del jugador.

**Características**:

- Detecta el ritmo del jugador con ESPACIO
- Calcula la latencia de entrada
- Avanza automáticamente a "CalibrationResultsScene"

### 3. CalibrationResultsController.cs

**Ubicación**: `Assets/Scripts/CalibrationResultsController.cs`

**Función**: Muestra los resultados de la calibración.

**Características**:

- Muestra la latencia calculada
- Avanza automáticamente a "MainMenuScene" con ESPACIO

### 4. MainMenuController.cs

**Ubicación**: `Assets/Scripts/MainMenuController.cs`

**Función**: Controla el menú principal del juego.

**Características**:

- Detecta ENTER o ESPACIO para avanzar a MainScene
- Métodos públicos para botones UI:
  - `GoToMainScene()`: Navega a la escena principal
  - `GoToCalibration()`: Vuelve a calibración
  - `PlayLevel(SongData)`: Inicia un nivel específico
  - `QuitGame()`: Cierra el juego

**Configuración**:

- `mainSceneName`: "MainScene"
- `gameSceneName`: "SampleScene"
- `calibrationSceneName`: "CalibrationScene"
- `optionsSceneName`: "OptionsScene"

**Input**:

- ENTER o ESPACIO: Avanza a MainScene

### 5. MainSceneController.cs ⭐ (NUEVO)

**Ubicación**: `Assets/Scripts/MainSceneController.cs`

**Función**: Controla la escena principal con opciones de navegación.

**Características**:

- Métodos públicos para botones UI:
  - `GoToOptions()`: Navega a la escena de opciones
  - `GoToPlay()`: Inicia el juego (SampleScene)
  - `GoToMainMenu()`: Vuelve al menú principal
  - `QuitGame()`: Cierra el juego

**Configuración**:

- `optionsSceneName`: "OptionsScene"
- `gameSceneName`: "SampleScene"

**Uso en Unity**:

1. Agregar el componente a un GameObject en MainScene
2. Crear botones UI con los textos "Play" y "Options"
3. Conectar los eventos OnClick:
   - Botón "Play" → `MainSceneController.GoToPlay()`
   - Botón "Options" → `MainSceneController.GoToOptions()`

## Sistema de Carga (SceneLoader)

Todos los controladores utilizan el patrón:

```csharp
if (SceneLoader.Instance != null)
    SceneLoader.Instance.LoadScene(sceneName);
else
    SceneManager.LoadScene(sceneName);
```

**Ventajas**:

- Transiciones suaves con fade
- Fallback automático si SceneLoader no existe
- Permite probar escenas individuales sin dependencias

## Escenas en Build Settings

El orden de las escenas en Build Settings es:

0. IntroScene
1. CalibrationScene
2. MainMenuScene
3. MainScene ⭐ (NUEVA)
4. OptionsScene ⭐ (NUEVA)
5. SampleScene

## Configuración en Unity

### MainScene

1. Crear un Canvas con botones UI
2. Agregar dos botones:
   - **Play**: Texto "play", conectar a `MainSceneController.GoToPlay()`
   - **Options**: Texto "options", conectar a `MainSceneController.GoToOptions()`
3. Agregar un GameObject vacío llamado "MainManager"
4. Agregar el componente `MainSceneController` al GameObject

### OptionsScene

- Escena básica con cámara
- Pendiente: Agregar controles de opciones (volumen, controles, etc.)
- Agregar botón "Back" que llame a `MainSceneController.GoToMainMenu()`

## Notas de Desarrollo

### Archivos Modificados

- `MainMenuController.cs`: Agregado método `Update()` para detectar ENTER/ESPACIO
- `EditorBuildSettings.asset`: Agregadas MainScene y OptionsScene

### Archivos Nuevos

- `MainSceneController.cs`: Controlador para la escena principal
- `MainScene.unity`: Escena con botones Play y Options
- `OptionsScene.unity`: Escena de opciones (placeholder)
- `FLUJO_ESCENAS.md`: Este documento

### Siguiente Paso

- Implementar la escena de opciones con controles de volumen
- Agregar sistema de selección de canciones en MainScene
- Agregar animaciones de transición entre escenas
