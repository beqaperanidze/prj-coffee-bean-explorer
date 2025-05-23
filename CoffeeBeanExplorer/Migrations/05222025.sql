CREATE TYPE "UserRole" AS ENUM ('User', 'Brewer', 'Admin');
CREATE TYPE "RoastLevel" AS ENUM ('Light', 'Medium', 'Dark');

CREATE SCHEMA IF NOT EXISTS "Product";
CREATE SCHEMA IF NOT EXISTS "Auth";
CREATE SCHEMA IF NOT EXISTS "Social";

CREATE TABLE "Product"."Tags"
(
    "Id"        SERIAL,
    "Name"      VARCHAR(50) NOT NULL,
    "CreatedAt" TIMESTAMP   NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP   NOT NULL DEFAULT now(),
    "CreatedBy" INT,
    "UpdatedBy" INT,
    CONSTRAINT "PK_Tags_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "UQ_Tags_Name" UNIQUE ("Name")
);

CREATE TABLE "Product"."Origins"
(
    "Id"        SERIAL,
    "Country"   VARCHAR(100) NOT NULL,
    "Region"    VARCHAR(100),
    "CreatedAt" TIMESTAMP    NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP    NOT NULL DEFAULT now(),
    CONSTRAINT "PK_Origins_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "UQ_Origins_Country_Region" UNIQUE ("Country", "Region")
);

CREATE TABLE "Product"."Beans"
(
    "Id"          SERIAL,
    "Name"        VARCHAR(100)   NOT NULL,
    "OriginId"    INT            NOT NULL,
    "RoastLevel"  "RoastLevel"   NOT NULL,
    "Description" VARCHAR(500),
    "Price"       DECIMAL(10, 2) NOT NULL,
    "CreatedAt"   TIMESTAMP      NOT NULL DEFAULT now(),
    "UpdatedAt"   TIMESTAMP      NOT NULL DEFAULT now(),
    CONSTRAINT "PK_Beans_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Beans_OriginId" FOREIGN KEY ("OriginId") REFERENCES "Product"."Origins" ("Id")
);

CREATE INDEX "IX_Beans_OriginId" ON "Product"."Beans" ("OriginId");
CREATE INDEX "IX_Beans_RoastLevel" ON "Product"."Beans" ("RoastLevel");
CREATE INDEX "IX_Beans_Name" ON "Product"."Beans" ("Name");

CREATE TABLE "Product"."BeansTags"
(
    "BeanId" INT NOT NULL,
    "TagId"  INT NOT NULL,
    CONSTRAINT "PK_BeansTags" PRIMARY KEY ("BeanId", "TagId"),
    CONSTRAINT "FK_BeansTags_BeanId" FOREIGN KEY ("BeanId") REFERENCES "Product"."Beans" ("Id"),
    CONSTRAINT "FK_BeansTags_TagId" FOREIGN KEY ("TagId") REFERENCES "Product"."Tags" ("Id")
);

CREATE TABLE "Auth"."Users"
(
    "Id"           SERIAL,
    "Username"     VARCHAR(50)  NOT NULL,
    "Email"        VARCHAR(255) NOT NULL,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FirstName"    VARCHAR(100),
    "LastName"     VARCHAR(100),
    "Role"         "UserRole"   NOT NULL DEFAULT 'User',
    "CreatedAt"    TIMESTAMP    NOT NULL DEFAULT now(),
    "UpdatedAt"    TIMESTAMP    NOT NULL DEFAULT now(),
    CONSTRAINT "PK_Users_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "UQ_Users_Username" UNIQUE ("Username"),
    CONSTRAINT "UQ_Users_Email" UNIQUE ("Email")
);

CREATE TABLE "Social"."Reviews"
(
    "Id"        SERIAL,
    "UserId"    INT       NOT NULL,
    "BeanId"    INT       NOT NULL,
    "Rating"    INT       NOT NULL,
    "Comment"   VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "PK_Reviews_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Reviews_UserId" FOREIGN KEY ("UserId") REFERENCES "Auth"."Users" ("Id"),
    CONSTRAINT "FK_Reviews_BeanId" FOREIGN KEY ("BeanId") REFERENCES "Product"."Beans" ("Id"),
    CONSTRAINT "CK_Reviews_Rating" CHECK ("Rating" BETWEEN 1 AND 5)
);

CREATE INDEX "IX_Reviews_UserId" ON "Social"."Reviews" ("UserId");
CREATE INDEX "IX_Reviews_BeanId" ON "Social"."Reviews" ("BeanId");
CREATE UNIQUE INDEX "IX_Reviews_UserId_BeanId" ON "Social"."Reviews" ("UserId", "BeanId");

CREATE TABLE "Social"."UserLists"
(
    "Id"        SERIAL,
    "UserId"    INT          NOT NULL,
    "Name"      VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP    NOT NULL DEFAULT now(),
    CONSTRAINT "PK_UserLists_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserLists_UserId" FOREIGN KEY ("UserId") REFERENCES "Auth"."Users" ("Id"),
    CONSTRAINT "UQ_UserLists_UserId_Name" UNIQUE ("UserId", "Name")
);

CREATE TABLE "Social"."ListItems"
(
    "ListId"    INT       NOT NULL,
    "BeanId"    INT       NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT now(),
    CONSTRAINT "PK_ListItems" PRIMARY KEY ("ListId", "BeanId"),
    CONSTRAINT "FK_ListItems_ListId" FOREIGN KEY ("ListId") REFERENCES "Social"."UserLists" ("Id"),
    CONSTRAINT "FK_ListItems_BeanId" FOREIGN KEY ("BeanId") REFERENCES "Product"."Beans" ("Id")
);
