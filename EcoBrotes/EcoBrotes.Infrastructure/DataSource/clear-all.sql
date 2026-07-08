-- Limpiar todas las tablas antes de sembrar datos.
-- TRUNCATE vacía las tablas de forma intencional (equivalente a un DELETE sin
-- WHERE, pero explícito y sin el riesgo que ese patrón implica), respeta las
-- claves foráneas con CASCADE y reinicia las secuencias con RESTART IDENTITY.
TRUNCATE TABLE
    "DetalleArbolJornada",
    "JornadaReforestacion",
    "EspecieArboreaEntity",
    "ZonaUrbanaEntity"
RESTART IDENTITY CASCADE;
