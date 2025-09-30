# simulador_final
Proyecto de simulador basado en agentes para entrega proyecto de tesis.

## Escena rápida (auto-setup)
1. Abre tu escena (por ejemplo `Assets/Scenes/SampleScene.unity`).
2. Crea un GameObject vacío `Bootstrap`.
3. Agrega los componentes:
	- `EscenaBootstrap` (auto-crea metas/salida/admin si faltan).
	- `UIBootstrap` (auto-crea Canvas y conecta `SimuladorUI`).
4. Bakea NavMesh del piso/terreno (Navigation > Bake).
5. Play. Verás agentes spawnándose hasta el aforo. Usa los botones (Iniciar, Detener, Reset, Exportar) y ajusta sliders (Aforo/Intervalo/Prob. de contagio). El resumen muestra agentes totales, infectados y %.

## Tags necesarios
- `tagPersonas`, `meta`, `salida_cc`, `muros` (opcional). Se intentan crear automáticamente en Editor.

## Exportación de reportes
- Por defecto (portable): `Application.persistentDataPath/ReporteAgentes/RptAgentes-<ddMMyyyyHHmmss>.json`.
- Alternativo: `C:\\ReporteAgentes\\...` si desactivas `usarRutaPortable` en `Administrador`.

## Notas
- Si no asignas prefab de agente al `Administrador`, se genera uno programáticamente con `NavMeshAgent`, `Camino`, `Particula` y `ParticleSystem`.
- Crea un `SimuladorConfig` (Create > Simulador > Config) si quieres centralizar parámetros.
