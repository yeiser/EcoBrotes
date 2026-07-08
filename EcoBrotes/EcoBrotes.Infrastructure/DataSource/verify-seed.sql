-- Verificar conteo de registros por tabla
SELECT 'ZonaUrbanaEntity' as tabla, COUNT(*) as total FROM "ZonaUrbanaEntity"
UNION ALL
SELECT 'EspecieArboreaEntity', COUNT(*) FROM "EspecieArboreaEntity"
UNION ALL
SELECT 'Customer', COUNT(*) FROM "Customer"
UNION ALL
SELECT 'Product', COUNT(*) FROM "Product"
UNION ALL
SELECT 'JornadaReforestacion', COUNT(*) FROM "JornadaReforestacion"
UNION ALL
SELECT 'DetalleArbolJornada', COUNT(*) FROM "DetalleArbolJornada"
ORDER BY tabla;
