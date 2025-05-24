CREATE TYPE "UserRole" AS ENUM ('User', 'Brewer', 'Admin');
CREATE TYPE "RoastLevel" AS ENUM ('Light', 'Medium', 'Dark');

CREATE SCHEMA IF NOT EXISTS "Product";
CREATE SCHEMA IF NOT EXISTS "Auth";
CREATE SCHEMA IF NOT EXISTS "Social";

CREATE TABLE "Product"."Tags"
(
    "Id"        SERIAL PRIMARY KEY,
    "Name"      VARCHAR(50) NOT NULL UNIQUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT now()
);

CREATE TABLE "Product"."Origins"
(
    "Id"        SERIAL PRIMARY KEY,
    "Country"   VARCHAR(100) NOT NULL,
    "Region"    VARCHAR(100),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "UQ_Origins_Country_Region" UNIQUE ("Country", "Region")
);

CREATE TABLE "Product"."Beans"
(
    "Id"          SERIAL PRIMARY KEY,
    "Name"        VARCHAR(100)   NOT NULL,
    "OriginId"    INT            NOT NULL REFERENCES "Product"."Origins" ("Id"),
    "RoastLevel"  "RoastLevel"   NOT NULL,
    "Description" VARCHAR(500),
    "Price"       DECIMAL(10, 2) NOT NULL,
    "CreatedAt"   TIMESTAMP      NOT NULL DEFAULT now(),
    "UpdatedAt"   TIMESTAMP      NOT NULL DEFAULT now()
);

CREATE INDEX "IX_Beans_OriginId" ON "Product"."Beans" ("OriginId");
CREATE INDEX "IX_Beans_RoastLevel" ON "Product"."Beans" ("RoastLevel");
CREATE INDEX "IX_Beans_Name" ON "Product"."Beans" ("Name");

CREATE TABLE "Product"."BeansTags"
(
    "BeanId"    INT NOT NULL REFERENCES "Product"."Beans" ("Id"),
    "TagId"     INT NOT NULL REFERENCES "Product"."Tags" ("Id"),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "PK_BeansTags" PRIMARY KEY ("BeanId", "TagId")
);

CREATE TABLE "Auth"."Users"
(
    "Id"           SERIAL PRIMARY KEY,
    "Username"     VARCHAR(50)  NOT NULL UNIQUE,
    "Email"        VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FirstName"    VARCHAR(100),
    "LastName"     VARCHAR(100),
    "Role"         "UserRole" NOT NULL DEFAULT 'User',
    "CreatedAt"    TIMESTAMP  NOT NULL DEFAULT now(),
    "UpdatedAt"    TIMESTAMP  NOT NULL DEFAULT now()
);

CREATE TABLE "Social"."Reviews"
(
    "Id"        SERIAL PRIMARY KEY,
    "UserId"    INT NOT NULL REFERENCES "Auth"."Users" ("Id"),
    "BeanId"    INT NOT NULL REFERENCES "Product"."Beans" ("Id"),
    "Rating"    INT NOT NULL CHECK ("Rating" BETWEEN 1 AND 5),
    "Comment"   VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT now()
);

CREATE INDEX "IX_Reviews_UserId" ON "Social"."Reviews" ("UserId");
CREATE INDEX "IX_Reviews_BeanId" ON "Social"."Reviews" ("BeanId");
CREATE UNIQUE INDEX "IX_Reviews_UserId_BeanId" ON "Social"."Reviews" ("UserId", "BeanId");

CREATE TABLE "Social"."UserLists"
(
    "Id"        SERIAL PRIMARY KEY,
    "UserId"    INT NOT NULL REFERENCES "Auth"."Users" ("Id"),
    "Name"      VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "UQ_UserLists_UserId_Name" UNIQUE ("UserId", "Name")
);

CREATE TABLE "Social"."ListItems"
(
    "ListId"    INT NOT NULL REFERENCES "Social"."UserLists" ("Id"),
    "BeanId"    INT NOT NULL REFERENCES "Product"."Beans" ("Id"),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "PK_ListItems" PRIMARY KEY ("ListId", "BeanId")
);
