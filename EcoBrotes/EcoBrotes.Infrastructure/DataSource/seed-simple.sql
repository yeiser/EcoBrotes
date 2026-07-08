-- Seed data for EcoBrotes
-- Run: psql -U postgres -d ecobrotess -f seed-simple.sql

-- 1. Zonas Urbanas
INSERT INTO "ZonaUrbanaEntity" ("Id", "Name", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'Zona Norte - Centro Comercial', NOW(), NOW()),
    (gen_random_uuid(), 'Zona Sur - Parque Metropolitano', NOW(), NOW()),
    (gen_random_uuid(), 'Zona Este - Unidad Educativa', NOW(), NOW()),
    (gen_random_uuid(), 'Zona Oeste - Comunidad Viva', NOW(), NOW()),
    (gen_random_uuid(), 'Zona Centro - Plaza Cívica', NOW(), NOW());

-- 2. Especies Arbóreas
INSERT INTO "EspecieArboreaEntity" ("Id", "Name", "ScientificName", "MaxHeightMeters", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'Mangle Rojo', 'Rhizophora mangle', 15.0, NOW(), NOW()),
    (gen_random_uuid(), 'Coco', 'Cocos nucifera', 20.0, NOW(), NOW()),
    (gen_random_uuid(), 'Tamarindo', 'Tamarindus indica', 12.0, NOW(), NOW()),
    (gen_random_uuid(), 'Guayacán', 'Handroanthus chrysanthus', 18.0, NOW(), NOW()),
    (gen_random_uuid(), 'Ceiba', 'Ceiba pentandra', 30.0, NOW(), NOW()),
    (gen_random_uuid(), 'Palo Rosa', 'Handroanthus impetiginosus', 25.0, NOW(), NOW());

-- 3. Clientes
INSERT INTO "Customer" ("Id", "Name", "TypeCustomer", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'María González', 1, NOW(), NOW()),
    (gen_random_uuid(), 'Carlos Pérez', 0, NOW(), NOW()),
    (gen_random_uuid(), 'Ana Martínez', 2, NOW(), NOW()),
    (gen_random_uuid(), 'Luis Ramírez', 1, NOW(), NOW()),
    (gen_random_uuid(), 'Sofía Torres', 0, NOW(), NOW()),
    (gen_random_uuid(), 'Pedro Castillo', 1, NOW(), NOW()),
    (gen_random_uuid(), 'Isabella Ruiz', 2, NOW(), NOW()),
    (gen_random_uuid(), 'Diego Flores', 1, NOW(), NOW());

-- 4. Productos
INSERT INTO "Product" ("Id", "Name", "ApplyIva", "Value", "CreatedOn", "LastModifiedOn") VALUES
    (gen_random_uuid(), 'Kit de Plantación', true, 25.50, NOW(), NOW()),
    (gen_random_uuid(), 'Bolsa de Abono Orgánico', true, 12.00, NOW(), NOW()),
    (gen_random_uuid(), 'Regadera Infantil', false, 18.75, NOW(), NOW()),
    (gen_random_uuid(), 'Guantes de Jardinería', true, 8.50, NOW(), NOW()),
    (gen_random_uuid(), 'Manual de Reforestación', true, 15.00, NOW(), NOW());

-- 5. Jornadas Reforestación
-- State: 0 = ConvocatoriaAbierta, 1 = EnProceso, 2 = Finalizada, 3 = Cancelada

-- ConvocatoriaAbierta
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Zona Norte', 'JOR-2026-001', z."Id", z."Id", DATE '2026-08-15', 50, 100, 0, NOW(), NOW()
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Norte - Centro Comercial';

INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Zona Sur', 'JOR-2026-002', z."Id", z."Id", DATE '2026-08-20', 75, 150, 0, NOW(), NOW()
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Sur - Parque Metropolitano';

INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Zona Este', 'JOR-2026-003', z."Id", z."Id", DATE '2026-08-25', 30, 60, 0, NOW(), NOW()
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Este - Unidad Educativa';

-- EnProceso
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Zona Oeste', 'JOR-2026-004', z."Id", z."Id", DATE '2026-07-10', 40, 80, 1, NOW() - INTERVAL '5 days', NOW()
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Oeste - Comunidad Viva';

INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Zona Centro', 'JOR-2026-005', z."Id", z."Id", DATE '2026-07-12', 60, 120, 1, NOW() - INTERVAL '3 days', NOW()
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Centro - Plaza Cívica';

-- Finalizada
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Norte Finalizada', 'JOR-2026-006', z."Id", z."Id", DATE '2026-06-15', 45, 90, 2, NOW() - INTERVAL '30 days', NOW() - INTERVAL '2 days'
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Norte - Centro Comercial';

INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Sur Finalizada', 'JOR-2026-007', z."Id", z."Id", DATE '2026-06-20', 55, 110, 2, NOW() - INTERVAL '25 days', NOW() - INTERVAL '3 days'
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Sur - Parque Metropolitano';

-- Cancelada
INSERT INTO "JornadaReforestacion" ("Id", "Name", "CodigoUnico", "ZonaId", "ZonaUrbanaId", "ScheduledDate", "TreeMeta", "VolunteerCapacity", "State", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), 'Jornada Este Cancelada', 'JOR-2026-008', z."Id", z."Id", DATE '2026-07-05', 20, 40, 3, NOW() - INTERVAL '10 days', NOW() - INTERVAL '2 days'
FROM "ZonaUrbanaEntity" z WHERE z."Name" = 'Zona Este - Unidad Educativa';

-- 6. Detalle Árboles para Jornadas EnProceso y Finalizadas

-- JOR-2026-004 (EnProceso)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), j."Id", e."Id", 15, NOW() - INTERVAL '4 days', NOW()
FROM "JornadaReforestacion" j, "EspecieArboreaEntity" e
WHERE j."CodigoUnico" = 'JOR-2026-004' AND e."Name" IN ('Mangle Rojo', 'Coco', 'Tamarindo');

-- JOR-2026-005 (EnProceso)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), j."Id", e."Id", 20, NOW() - INTERVAL '2 days', NOW()
FROM "JornadaReforestacion" j, "EspecieArboreaEntity" e
WHERE j."CodigoUnico" = 'JOR-2026-005' AND e."Name" IN ('Guayacán', 'Ceiba', 'Palo Rosa');

-- JOR-2026-006 (Finalizada)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), j."Id", e."Id", 15, NOW() - INTERVAL '28 days', NOW() - INTERVAL '2 days'
FROM "JornadaReforestacion" j, "EspecieArboreaEntity" e
WHERE j."CodigoUnico" = 'JOR-2026-006' AND e."Name" IN ('Mangle Rojo', 'Guayacán', 'Tamarindo');

-- JOR-2026-007 (Finalizada)
INSERT INTO "DetalleArbolJornada" ("Id", "JornadaReforestacionId", "EspecieArboreaId", "Quantity", "CreatedOn", "LastModifiedOn")
SELECT gen_random_uuid(), j."Id", e."Id", 18, NOW() - INTERVAL '23 days', NOW() - INTERVAL '3 days'
FROM "JornadaReforestacion" j, "EspecieArboreaEntity" e
WHERE j."CodigoUnico" = 'JOR-2026-007' AND e."Name" IN ('Coco', 'Ceiba', 'Palo Rosa');
