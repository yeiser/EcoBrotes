-- =====================================================
-- Seed Data for EcoBrotes (PostgreSQL)
-- Database: ecobrotess
-- =====================================================

-- 1. Zonas Urbanas
INSERT INTO "ZonaUrbanaEntity" ("Id", "Name") VALUES
    (gen_random_uuid(), 'Zona Norte - Centro Comercial'),
    (gen_random_uuid(), 'Zona Sur - Parque Metropolitano'),
    (gen_random_uuid(), 'Zona Este - Unidad Educativa'),
    (gen_random_uuid(), 'Zona Oeste - Comunidad Viva'),
    (gen_random_uuid(), 'Zona Centro - Plaza Cívica');

-- 2. Especies Arbóreas
INSERT INTO "EspecieArboreaEntity" ("Id", "Name", "ScientificName", "MaxHeightMeters") VALUES
    (gen_random_uuid(), 'Mangle Rojo', 'Rhizophora mangle', 15.0),
    (gen_random_uuid(), 'Coco', 'Cocos nucifera', 20.0),
    (gen_random_uuid(), 'Tamarindo', 'Tamarindus indica', 12.0),
    (gen_random_uuid(), 'Guayacán', 'Handroanthus chrysanthus', 18.0),
    (gen_random_uuid(), 'Ceiba', 'Ceiba pentandra', 30.0),
    (gen_random_uuid(), 'Palo Rosa', 'Handroanthus impetiginosus', 25.0);

-- 3. Tipos de Cliente
-- (Los tipos son enum: Preferential, Common, Special - no se guardan como filas)

-- 4. Clientes
INSERT INTO "Customer" ("Id", "Name", "TypeCustomer", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'María González', 1, NOW(), NOW()),    -- Common
    (gen_random_uuid(), 'Carlos Pérez', 0, NOW(), NOW()),       -- Preferential
    (gen_random_uuid(), 'Ana Martínez', 2, NOW(), NOW()),       -- Special
    (gen_random_uuid(), 'Luis Ramírez', 1, NOW(), NOW()),       -- Common
    (gen_random_uuid(), 'Sofía Torres', 0, NOW(), NOW()),       -- Preferential
    (gen_random_uuid(), 'Pedro Castillo', 1, NOW(), NOW()),     -- Common
    (gen_random_uuid(), 'Isabella Ruiz', 2, NOW(), NOW()),      -- Special
    (gen_random_uuid(), 'Diego Flores', 1, NOW(), NOW());      -- Common

-- 5. Productos
INSERT INTO "Product" ("Id", "Name", "ApplyIva", "Value", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'Kit de Plantación', true, 25.50, NOW(), NOW()),
    (gen_random_uuid(), 'Bolsa de Abono Orgánico', true, 12.00, NOW(), NOW()),
    (gen_random_uuid(), 'Regadera Infantil', false, 18.75, NOW(), NOW()),
    (gen_random_uuid(), 'Guantes de Jardinería', true, 8.50, NOW(), NOW()),
    (gen_random_uuid(), 'Manual de Reforestación', true, 15.00, NOW(), NOW());

-- 6. Jornadas Reforestación - Estado: ConvocatoriaAbierta (State = 0)
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Norte', 'JOR-2026-001', z."Id", z."Id", DATE '2026-08-15', 50, 100, 0, NOW(), NOW() FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Norte - Centro Comercial'
UNION ALL
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Sur', 'JOR-2026-002', z."Id", z."Id", DATE '2026-08-20', 75, 150, 0, NOW(), NOW() FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Sur - Parque Metropolitano'
UNION ALL
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Este', 'JOR-2026-003', z."Id", z."Id", DATE '2026-08-25', 30, 60, 0, NOW(), NOW() FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Este - Unidad Educativa';

-- 7. Jornadas Reforestación - Estado: EnProceso (State = 1)
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Oeste', 'JOR-2026-004', z."Id", z."Id", DATE '2026-07-10', 40, 80, 1, NOW() - INTERVAL '5 days', NOW() FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Oeste - Comunidad Viva'
UNION ALL
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Centro', 'JOR-2026-005', z."Id", z."Id", DATE '2026-07-12', 60, 120, 1, NOW() - INTERVAL '3 days', NOW() FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Centro - Plaza Cívica';

-- 8. Jornadas Reforestación - Estado: Finalizada (State = 2)
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Norte Finalizada', 'JOR-2026-006', z."Id", z."Id", DATE '2026-06-15', 45, 90, 2, NOW() - INTERVAL '30 days', NOW() - INTERVAL '2 days' FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Norte - Centro Comercial'
UNION ALL
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Sur Finalizada', 'JOR-2026-007', z."Id", z."Id", DATE '2026-06-20', 55, 110, 2, NOW() - INTERVAL '25 days', NOW() - INTERVAL '3 days' FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Sur - Parque Metropolitano';

-- 9. Jornadas Reforestación - Estado: Cancelada (State = 3)
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Reforestación Zona Este Cancelada', 'JOR-2026-008', z."Id", z."Id", DATE '2026-07-05', 20, 40, 3, NOW() - INTERVAL '10 days', NOW() - INTERVAL '2 days' FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Este - Unidad Educativa';

-- 10. Detalle de Árboles por Jornada (solo para jornadas EnProceso y Finalizadas)
-- Jornada JOR-2026-004 (EnProceso)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT
    gen_random_uuid(),
    (SELECT "Id" FROM "JornadaReforestacion" WHERE "CodigoUnico" = 'JOR-2026-004' LIMIT 1),
    e."Id",
    CASE
        WHEN e."Name" = 'Mangle Rojo' THEN 15
        WHEN e."Name" = 'Coco' THEN 10
        WHEN e."Name" = 'Tamarindo' THEN 15
    END,
    NOW(),
    NOW()
FROM "EspecieArboreaEntity" e
WHERE e."Name" IN ('Mangle Rojo', 'Coco', 'Tamarindo');

-- Jornada JOR-2026-005 (EnProceso)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT
    gen_random_uuid(),
    (SELECT "Id" FROM "JornadaReforestacion" WHERE "CodigoUnico" = 'JOR-2026-005' LIMIT 1),
    e."Id",
    CASE
        WHEN e."Name" = 'Guayacán' THEN 20
        WHEN e."Name" = 'Ceiba' THEN 20
        WHEN e."Name" = 'Palo Rosa' THEN 20
    END,
    NOW(),
    NOW()
FROM "EspecieArboreaEntity" e
WHERE e."Name" IN ('Guayacán', 'Ceiba', 'Palo Rosa');

-- Jornada JOR-2026-006 (Finalizada)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT
    gen_random_uuid(),
    (SELECT "Id" FROM "JornadaReforestacion" WHERE "CodigoUnico" = 'JOR-2026-006' LIMIT 1),
    e."Id",
    CASE
        WHEN e."Name" = 'Mangle Rojo' THEN 20
        WHEN e."Name" = 'Guayacán' THEN 15
        WHEN e."Name" = 'Tamarindo' THEN 10
    END,
    NOW(),
    NOW()
FROM "EspecieArboreaEntity" e
WHERE e."Name" IN ('Mangle Rojo', 'Guayacán', 'Tamarindo');

-- Jornada JOR-2026-007 (Finalizada)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT
    gen_random_uuid(),
    (SELECT "Id" FROM "JornadaReforestacion" WHERE "CodigoUnico" = 'JOR-2026-007' LIMIT 1),
    e."Id",
    CASE
        WHEN e."Name" = 'Coco' THEN 25
        WHEN e."Name" = 'Ceiba' THEN 15
        WHEN e."Name" = 'Palo Rosa' THEN 15
    END,
    NOW(),
    NOW()
FROM "EspecieArboreaEntity" e
WHERE e."Name" IN ('Coco', 'Ceiba', 'Palo Rosa');

-- =====================================================
-- Resumen de datos insertados:
-- Zonas Urbanas: 5
-- Especies Arbóreas: 6
-- Clientes: 8
-- Productos: 5
-- Jornadas Reforestación: 8 (4 ConvocatoriaAbierta, 2 EnProceso, 2 Finalizada, 1 Cancelada)
-- Detalle Árboles: ~12 registros
-- =====================================================
