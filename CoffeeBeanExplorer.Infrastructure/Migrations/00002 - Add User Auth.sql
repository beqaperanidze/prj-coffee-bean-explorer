CREATE TABLE "Auth"."RefreshTokens"
(
    "Id"              SERIAL,
    "Token"           VARCHAR(500) NOT NULL,
    "Expires"         TIMESTAMP    NOT NULL,
    "Created"         TIMESTAMP    NOT NULL DEFAULT now(),
    "Revoked"         TIMESTAMP,
    "ReplacedByToken" VARCHAR(500),
    "ReasonRevoked"   VARCHAR(255),
    "UserId"          INT          NOT NULL,
    CONSTRAINT "PK_RefreshTokens_Id" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RefreshTokens_UserId" FOREIGN KEY ("UserId") REFERENCES "Auth"."Users" ("Id")
);

ALTER TABLE "Auth"."Users"
    ADD COLUMN "Salt"      VARCHAR(255),
    ADD COLUMN "LastLogin" TIMESTAMP,
    ADD COLUMN "IsActive"  BOOLEAN NOT NULL DEFAULT true;
