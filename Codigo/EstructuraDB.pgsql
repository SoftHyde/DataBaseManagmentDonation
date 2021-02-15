--Este script es usado para realizar la creacion de tablas y relaciones de la Base de datos (DB)
--Nombre DB: proyecto_analista

/* Orden de creacion de las tablas:
    Grupo 1: Producto
             Tipo
             Dueño
             Negocio
             Voluntario
    Grupo 2: Donacion
             Ubicacion
    Grupo 3: Asociacion_Ubicacion
    Tablas auxiliares
*/

/* Concideraciones:
    -Las Primaty Key se crearon con el esquema pk_NOMBRETABLA
    -Las foreing key se crearon con el esquema fk#_NOMBRETABLA_NOMBREREF
    -Las Uniques se crearon con el esquema uk_NOMBRETABLA
    -Formato de fecha: AA/MM/DD
*/

--Grupo 1
CREATE TABLE Producto
(
    ID_Producto         INT8        NOT NULL,
    Nombre              VARCHAR     NOT NULL,
    CONSTRAINT pk_Producto          PRIMARY KEY (ID_Producto)
);

CREATE TABLE Tipo
(
    ID_Tipo             INT8        NOT NULL,
    Nombre              VARCHAR     NOT NULL,
    CONSTRAINT pk_Tipo              PRIMARY KEY (ID_Tipo),
    CONSTRAINT CHK_Nombre           CHECK (Nombre IN ('piso','puesto'))
);

INSERT INTO Tipo VALUES ('1','piso'), ('2','puesto');

CREATE TABLE Duenio
(
    ID_Duenio           INT8        NOT NULL,
    Nombre              VARCHAR     NOT NULL,
    CONSTRAINT pk_Duenio            PRIMARY KEY (ID_Duenio)
);

CREATE TABLE Negocio
(
    ID_Negocio          INT8        NOT NULL,
    Nombre              VARCHAR     NOT NULL,
    Mail                VARCHAR     NULL,
    Telefono            VARCHAR     NULL,
    CONSTRAINT pk_Negocio           PRIMARY KEY (ID_Negocio)
);

CREATE TABLE Voluntario
(
    ID_Voluntario       INT8        NOT NULL,
    Nombre              VARCHAR     NOT NULL,
    DNI                 VARCHAR     NOT NULL,
    Es_Admin            BOOLEAN     NOT NULL,
    Pass                VARCHAR     NOT NULL,
    CONSTRAINT pk_Voluntario        PRIMARY KEY (ID_Voluntario),
    CONSTRAINT uk1_Voluntario       UNIQUE (DNI),
    CONSTRAINT uk2_Voluntario       UNIQUE (Nombre)
);

--Grupo 2
CREATE TABLE Donacion
(
    ID_Donacion         INT8        NOT NULL,
    ID_Producto         INT8        NOT NULL,
    ID_Negocio          INT8        NOT NULL,
    ID_Voluntario1      INT8        NOT NULL,
    ID_Voluntario       INT8        NOT NULL, --este ID_Voluntario que no tiene numero refiere al usuario que registro la donacion
    Peso                REAL        NOT NULL CHECK (Peso >= 0),
    Fecha               DATE        NOT NULL,
    ID_Voluntario2      INT8        NULL,
    ID_Voluntario3      INT8        NULL,
    ID_Voluntario4      INT8        NULL,
    CONSTRAINT pk_Donacion          PRIMARY KEY (ID_Donacion),
    CONSTRAINT fk1_Donacion_Producto   FOREIGN KEY (ID_Producto) REFERENCES Producto (ID_Producto),
    CONSTRAINT fk2_Donacion_Negocio    FOREIGN KEY (ID_Negocio)  REFERENCES Negocio (ID_Negocio),
    CONSTRAINT fk3_Donacion_Voluntario FOREIGN KEY (ID_Voluntario1)  REFERENCES Voluntario (ID_Voluntario),
    CONSTRAINT fk4_Donacion_Voluntario FOREIGN KEY (ID_Voluntario2)  REFERENCES Voluntario (ID_Voluntario),
    CONSTRAINT fk5_Donacion_Voluntario FOREIGN KEY (ID_Voluntario3)  REFERENCES Voluntario (ID_Voluntario),
    CONSTRAINT fk6_Donacion_Voluntario FOREIGN KEY (ID_Voluntario4)  REFERENCES Voluntario (ID_Voluntario),
    CONSTRAINT fk7_Donacion_Voluntario FOREIGN KEY (ID_Voluntario)  REFERENCES Voluntario (ID_Voluntario),
    CONSTRAINT uk_Donacion             UNIQUE (Fecha,ID_Producto,ID_Negocio)
);

CREATE TABLE Ubicacion
(
    ID_Ubicacion        INT8        NOT NULL,
    ID_Tipo             INT8        NOT NULL,
    Numero              INTEGER     NOT NULL CHECK (Numero > 0),
    ID_Duenio           INT8        NULL,
    CONSTRAINT pk_Ubicacion         PRIMARY KEY (ID_Ubicacion),
    CONSTRAINT fk1_Ubicacion_Tipo   FOREIGN KEY (ID_Tipo) REFERENCES Tipo (ID_Tipo),
    CONSTRAINT fk4_Ubicacion_Duenio FOREIGN KEY (ID_Duenio) REFERENCES Duenio (ID_Duenio),
    CONSTRAINT uk_Ubicacion         UNIQUE (Numero, ID_Tipo)
);

--Grupo 3
CREATE TABLE Asociacion_Ubicacion
(
    ID_Asociacion       INT8        NOT NULL,
    ID_Ubicacion        INT8        NOT NULL,
    ID_Donacion         INT8        NOT NULL,
    ID_Negocio          INT8        NOT NULL,
    CONSTRAINT pk_Asociacion_Ubicacion            PRIMARY KEY (ID_Asociacion),
    CONSTRAINT fk1_Asociacion_Ubicacion_Ubicacion FOREIGN KEY (ID_Ubicacion) REFERENCES Ubicacion (ID_Ubicacion),
    CONSTRAINT fk2_Asociacion_Ubicacion_Donacion  FOREIGN KEY (ID_Donacion) REFERENCES Donacion (ID_Donacion),
    CONSTRAINT fk3_Asociacion_Ubicacion_Negocio   FOREIGN KEY (ID_Negocio) REFERENCES Negocio (ID_Negocio),
    CONSTRAINT uk_Asociacion_Ubicacion            UNIQUE (ID_Ubicacion, ID_Donacion, ID_Negocio)
);

--Tablas Auxiliares
CREATE TABLE Auxiliar
(
    Fecha       DATE,
    Vol1        VARCHAR,
    Vol2        VARCHAR,
    Vol3        VARCHAR,
    Vol4        VARCHAR,
    nomb_neg    VARCHAR,
    numero      INTEGER,
    P_o_P       VARCHAR,
    telefono    VARCHAR,
    mail        VARCHAR,
    duenio      VARCHAR,
    producto    VARCHAR,
    peso        REAL
);

CREATE TABLE Auxiliar_ID
(
    ID_1     INT8
);

/* Luego de crear todas las tablas, ingresar el siguiente comando */
CREATE EXTENSION pgcrypto;
/*Este crea la extension en el BD para llevar a cabo una encriptacion en los campos de contraseña de los voluntarios*/