-- Tablas demo (código existente)
CREATE TABLE IF NOT EXISTS "Customer" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "TypeCustomer" SMALLINT NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "Product" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "ApplyIva" BOOLEAN NOT NULL,
    "Value" NUMERIC(18,2) NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "Invoice" (
    "Id" UUID PRIMARY KEY,
    "CustomerId" UUID NOT NULL REFERENCES "Customer"("Id") ON DELETE CASCADE,
    "ValueTotal" NUMERIC(18,2) NOT NULL,
    "State" SMALLINT NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ProductInvoice" (
    "Id" UUID PRIMARY KEY,
    "Quantity" NUMERIC(18,2) NOT NULL,
    "InvoiceId" UUID NOT NULL REFERENCES "Invoice"("Id") ON DELETE CASCADE,
    "ProductId" UUID NOT NULL REFERENCES "Product"("Id") ON DELETE CASCADE,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "IX_Invoice_CustomerId" ON "Invoice"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_ProductInvoice_InvoiceId" ON "ProductInvoice"("InvoiceId");
CREATE INDEX IF NOT EXISTS "IX_ProductInvoice_ProductId" ON "ProductInvoice"("ProductId");

-- Tablas EcoBrotes
CREATE TABLE IF NOT EXISTS "ZonaUrbana" (
    "Id" UUID PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL,
    "ComunaLocalidad" VARCHAR(100) NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "EspecieArborea" (
    "Id" UUID PRIMARY KEY,
    "NombreComun" VARCHAR(200) NOT NULL,
    "NombreCientifico" VARCHAR(200) NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "JornadaReforestacion" (
    "Id" UUID PRIMARY KEY,
    "CodigoUnico" VARCHAR(50) NOT NULL UNIQUE,
    "NombreJornada" VARCHAR(200) NOT NULL,
    "FechaHora" TIMESTAMP WITH TIME ZONE NOT NULL,
    "MetaArbolesTotal" INTEGER NOT NULL,
    "CupoVoluntarios" INTEGER NOT NULL,
    "TotalInscritos" INTEGER NOT NULL DEFAULT 0,
    "Estado" VARCHAR(50) NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "DetalleArbolJornada" (
    "Id" UUID PRIMARY KEY,
    "JornadaId" UUID NOT NULL REFERENCES "JornadaReforestacion"("Id") ON DELETE CASCADE,
    "EspecieId" UUID NOT NULL REFERENCES "EspecieArborea"("Id") ON DELETE CASCADE,
    "CantidadAsignada" INTEGER NOT NULL,
    "CreatedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "LastModifiedOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "IX_JornadaReforestacion_CodigoUnico" ON "JornadaReforestacion"("CodigoUnico");
CREATE INDEX IF NOT EXISTS "IX_JornadaReforestacion_FechaHora" ON "JornadaReforestacion"("FechaHora");
CREATE INDEX IF NOT EXISTS "IX_JornadaReforestacion_Estado" ON "JornadaReforestacion"("Estado");
CREATE INDEX IF NOT EXISTS "IX_DetalleArbolJornada_JornadaId" ON "DetalleArbolJornada"("JornadaId");
CREATE INDEX IF NOT EXISTS "IX_DetalleArbolJornada_EspecieId" ON "DetalleArbolJornada"("EspecieId");
