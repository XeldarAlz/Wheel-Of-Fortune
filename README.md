## Core Features

- **Progressive Zone System**: Three difficulty zones (Bronze, Silver, Gold) with increasing rewards and bomb chances
- **Wheel Spinning Mechanics**: Smooth DOTween-based animations with customizable spin outcomes
- **Bomb & Revival System**: Strategic risk-reward gameplay with revival options
- **Reward Collection**: Track and collect multiple reward types (currency, items)
- **Haptic Feedback**: Platform-specific vibration patterns (iOS/Android)
- **Audio System**: Dynamic music and SFX with fade transitions
- **Cheat System**: Development tools for testing specific outcomes and zones

## Architecture

The project follows a **clean architecture** approach with clear separation of concerns:

```
Assets/_Game/Scripts/
├── View/              # UI components and visual presentation
├── Presentation/      # Presenters, adapters, and installers
├── Infrastructure/    # Core services, DI, factories, interfaces
├── Data/              # ScriptableObjects and data models
└── Core/              # Business logic, services, state machine
```

**Key Layers:**
- **View Layer**: MonoBehaviour components for UI/visual elements
- **Presentation Layer**: Mediates between View and Core using Presenter pattern
- **Core Layer**: Game logic, state management, and business rules
- **Infrastructure Layer**: Cross-cutting concerns (DI, pooling, factories)
- **Data Layer**: Configuration and runtime data models

## Design Patterns

### Creational Patterns
- **Factory Pattern**: `GameFactory` for creating game context and dependencies
- **Object Pooling**: `RewardItemPool` for efficient reward view management

### Structural Patterns
- **Adapter Pattern**: `WheelViewAdapter`, `WheelSpinServiceAdapter` for interface adaptation
- **Facade Pattern**: `ScreenServiceFacade` for simplified screen management

### Behavioral Patterns
- **State Machine**: `WheelGameStateMachine` with `PlayingState` and `BombState`
- **Strategy Pattern**: Zone-specific strategies (`BronzeZoneStrategy`, `SilverZoneStrategy`, `SuperZoneStrategy`)
- **Observer Pattern**: Event-driven communication between View and Presenters

### Architectural Patterns
- **Service Locator**: `ServiceLocator` for dependency resolution
- **Dependency Injection**: Constructor injection throughout the core services
- **MVP (Model-View-Presenter)**: Clear separation between UI and business logic
