# RithmSlash

Un juego de ritmo y acción en 2D inspirado en Metroidvania donde cada acción debe ejecutarse al compás de la música.

---

## ¿Qué es?

**RithmSlash** es un juego de plataformas 2D que combina:

- **Mecánicas rítmicas**: Todo movimiento, salto y ataque debe sincronizarse con el beat de la música
- **Exploración estilo Metroidvania**: Mapa interconectado con diferentes zonas y enemigos
- **Sistema de combate**: Ataque, parry de proyectiles y gestión de stamina
- **Feedback visual**: Indicadores en tiempo real de precisión rítmica

Inspirado en **Crypt of the NecroDancer**, cada input se valida contra el Conductor musical que detecta beats y sincroniza toda la lógica del juego.

---

## Requisitos

- **Unity** 6000.2.13f1 o superior
- **Sistema Operativo**: Windows/Mac/Linux
- **Dependencias**:
  - DOTween (incluido en `Assets/Plugins/Demigiant`)
  - TextMesh Pro (incluido)
  - Unity Input System (configurado en `InputSystem_Actions.inputactions`)

---

## Quickstart

### Instalar

1. Clona el repositorio:

   ```bash
   git clone https://github.com/tu-usuario/rithmslash-game.git
   cd rithmslash-game
   ```

2. Abre el proyecto con **Unity Hub**:
   - Añade el proyecto desde la carpeta raíz
   - Asegúrate de usar Unity 6000.2.13f1 o compatible

3. Unity descargará automáticamente los paquetes necesarios al abrir el proyecto

### Configurar

1. Abre la escena principal: [Assets/Scenes/SampleScene.unity](Assets/Scenes/SampleScene.unity) o [MainMenuScene.unity](Assets/Scenes/MainMenuScene.unity)

2. **Configurar música**:
   - Ve a [Assets/Data](Assets/Data) y selecciona un `SongData` (ej: `test_song.asset`)
   - Ajusta el **BPM** y **firstBeatOffset** según tu canción
   - Asigna el `SongData` al objeto `Conductor` en la escena

3. **Ajustar tolerancia rítmica** (opcional):
   - Selecciona el objeto `RhythmInput` en la jerarquía
   - Modifica `toleranceSeconds` (ventana de éxito) y `bufferTimeBeforeBeat` (input anticipado)

### Ejecutar

1. Presiona **Play** en el editor de Unity
2. Usa los controles:
   - **A/D** o **Flechas**: Mover (en el beat)
   - **W** o **Flecha arriba**: Saltar
   - **Q/E**: Atacar izquierda/derecha
   - **S** o **Flecha abajo**: Parry (bloquear proyectiles)
   - **Espacio**: Agarrarse a paredes

3. Observa el indicador visual de beats en la interfaz para sincronizar tus acciones

---

## Configuración

### Variables Importantes

#### Conductor ([Assets/Scripts/Scenes/Conductor.cs](Assets/Scripts/Scenes/Conductor.cs))

- `currentSongData`: Asigna un ScriptableObject SongData con el clip de audio y BPM
- `inputOffset`: Calibración de latencia (0 por defecto, ajustar si hay delay)
- `loopSong`: Si la música se repite automáticamente

#### RhythmInput ([Assets/Scripts/Rhythm/RhythmInput.cs](Assets/Scripts/Rhythm/RhythmInput.cs))

- `toleranceSeconds`: Ventana de tiempo para inputs exitosos (0.25s recomendado)
- `bufferTimeBeforeBeat`: Cuánto antes se puede presionar (0.15s)
- `minAccuracyThreshold`: Precisión mínima para considerarse válido (0-1)

#### PlayerCombat ([Assets/Scripts/Player/PlayerCombat.cs](Assets/Scripts/Player/PlayerCombat.cs))

- `maxHealth`: Vida del jugador
- `maxParryCharges`: Cantidad de parries disponibles
- `parryChargeRegenTime`: Tiempo para regenerar un parry
- `attackDamage`: Daño base de ataques

#### PlayerController ([Assets/Scripts/Player/PlayerController.cs](Assets/Scripts/Player/PlayerController.cs))

- `standardStride`: Distancia de movimiento por beat
- `jumpForce`: Fuerza del salto
- `maxStamina`: Stamina máxima para agarrarse a paredes
- `staminaDecreaseRate`: Velocidad de consumo de stamina

---

## Uso

### Crear Nuevas Canciones

1. Crea un nuevo `SongData`:
   - Clic derecho en [Assets/Data](Assets/Data) → Create → Rhythm Game → Song Data
   - Asigna tu AudioClip
   - Configura el BPM (usa un detector de BPM online si no lo conoces)
   - Ajusta `firstBeatOffset` si la canción no empieza en el segundo 0

2. Asigna el SongData al Conductor en tu escena

### Añadir Nuevos Enemigos

1. Crea un script que herede de `EnemyBase` (ver [Assets/Scripts/Enemy/EnemyBase.cs](Assets/Scripts/Enemy/EnemyBase.cs))
2. Implementa el método abstracto `PerformRhythmAction()`:

   ```csharp
   protected override void PerformRhythmAction()
   {
       // Acción que se ejecuta cada X beats (definido por actionInterval)
   }
   ```

3. Configura `actionInterval` para controlar cada cuántos beats actúa el enemigo

### Cambiar Música por Zonas

1. Añade el componente `MusicChangeZone` a un objeto con Collider2D (trigger)
2. Asigna `zoneMusic` y `zoneBpm`
3. La música cambiará cuando el jugador entre en la zona

---

## Estructura

```txt
rithmslash-game/
├── Assets/
│   ├── Scripts/
│   │   ├── Rhythm/          # Sistema de entrada rítmica y validación
│   │   │   ├── RhythmInput.cs         # Detecta inputs y valida timing
│   │   │   ├── RhythmicProjectile.cs  # Proyectiles que se pueden parry
│   │   │   └── MusicChangeZone.cs     # Cambio de música por zonas
│   │   ├── Scenes/          # Lógica de escenas y Conductor
│   │   │   └── Conductor.cs           # Corazón del sistema rítmico
│   │   ├── Player/          # Mecánicas del jugador
│   │   │   ├── PlayerController.cs    # Movimiento y salto
│   │   │   └── PlayerCombat.cs        # Combate, parry y vida
│   │   ├── Enemy/           # Enemigos y jefes
│   │   │   ├── EnemyBase.cs           # Clase base para enemigos
│   │   │   ├── PatrolEnemy.cs         # Enemigo que patrulla
│   │   │   └── BossWolf.cs            # Boss con ataques especiales
│   │   └── Effects/         # Efectos visuales sincronizados
│   ├── Data/               # ScriptableObjects de canciones
│   ├── Scenes/             # Escenas del juego
│   ├── Prefabs/            # Enemigos, proyectiles, UI
│   ├── Audio/              # Música y efectos de sonido
│   └── Sprites/            # Arte y animaciones
└── ProjectSettings/        # Configuración de Unity
```

### Archivos Clave

- [Assets/Scripts/Scenes/Conductor.cs](Assets/Scripts/Scenes/Conductor.cs): Calcula la posición de la canción en beats y dispara eventos `OnBeat`
- [Assets/Scripts/Rhythm/RhythmInput.cs](Assets/Scripts/Rhythm/RhythmInput.cs): Valida si los inputs del jugador están sincronizados
- [Assets/Scripts/Others/SongData.cs](Assets/Scripts/Others/SongData.cs): ScriptableObject para datos de canciones
- [Assets/Scripts/Player/PlayerCombat.cs](Assets/Scripts/Player/PlayerCombat.cs): Sistema de combate y parry
- [Assets/Scripts/Enemy/EnemyBase.cs](Assets/Scripts/Enemy/EnemyBase.cs): Clase base para comportamiento rítmico de enemigos

---

## Problemas Comunes

### La música está desfasada con los beats

**Solución**: Ajusta el `firstBeatOffset` en el SongData. Valores positivos retrasan la música, negativos la adelantan.

### Los inputs no se registran

**Solución**:

- Verifica que el objeto `RhythmInput` esté activo en la escena
- Aumenta `toleranceSeconds` si la ventana es muy estricta
- Revisa que el Conductor esté reproduciendo música (`musicSource.isPlaying`)

### El jugador se mueve sin sincronizar

**Solución**: Asegúrate de que todos los inputs se ejecutan a través del sistema de `RhythmInput`, no directamente en Update()

### Los enemigos no actúan

**Solución**:

- Verifica que `Conductor.Instance.OnBeat` está conectado (se hace automáticamente en `EnemyBase.Start()`)
- Revisa el valor de `actionInterval` (1 = cada beat, 2 = cada 2 beats, etc.)

### Error: "DOTween not found"

**Solución**: Importa DOTween desde la carpeta [Assets/Plugins/Demigiant](Assets/Plugins/Demigiant) o descárgalo desde el Asset Store

### La UI de beats no se muestra

**Solución**: Asegúrate de tener el prefab `BeatLine` o `ClockHUD` en la escena y conectado al evento `OnBeat` del Conductor

---

## Créditos

### Música y Efectos de Sonido

- [Nivel 1: Plains](https://www.newgrounds.com/audio/listen/588838)
- [Nivel 1: Cueva](https://www.newgrounds.com/audio/listen/577625)
- [Nivel 1: Boss](https://www.newgrounds.com/audio/listen/551069)
- [Kenney UI Audio](https://kenney.nl/assets/ui-audio)
- [Universal UI Soundpack por Cyrex Studios](https://cyrex-studios.itch.io/universal-ui-soundpack)

### Sprites y Arte

- [Super Asset Bundle 5 Mini Pocket Status por HumblePixel](https://humblepixel.itch.io/super-asset-bundle-5-mini-pocket-status)
- [2D Pixel Art Wolf Sprites por Elthen](https://elthen.itch.io/2d-pixel-art-wolf-sprites)
- [Slime Animations Pixel Art 2D por EduardScarpato](https://eduardscarpato.itch.io/slime-animations-pixel-art-2d)
- [Health Progress Series 2 Hearts por HumblePixel](https://humblepixel.itch.io/health-progress-series-2-hearts)
- [Free Pixel Effects Pack 10 Mini Magick Shoots2 por XyeZawr](https://xyezawr.itch.io/free-pixel-effects-pack-10-mini-magick-shoots2)
- [Free Platformer 16x16 por OisouGabo](https://oisougabo.itch.io/free-platformer-16x16)
- [Basic 140 Tiles Grassland and Mines por Anokolisa](https://anokolisa.itch.io/basic-140-tiles-grassland-and-mines)
- [Character Satyr por Lucky Loops](https://lucky-loops.itch.io/character-satyr)
