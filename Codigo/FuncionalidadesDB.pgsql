--Este script es usado para realizar la creacion de funciones de la DB
--Nombre DB: proyecto_analista

--------------------Funciones--------------------
--Insercion
CREATE OR REPLACE FUNCTION Insertar_donacion
(
    IN voluntario1 Voluntario.Nombre%TYPE,
    IN nomb_prod Producto.Nombre%TYPE,
    IN peso_prod Donacion.Peso%TYPE,
    IN puesto_o_piso Tipo.Nombre%TYPE,
    IN nom_user Voluntario.DNI%TYPE,
    IN numer Ubicacion.Numero%TYPE,
    IN nomb_negocio Negocio.Nombre%TYPE,
    IN voluntario2 Voluntario.Nombre%TYPE DEFAULT NULL,
    IN voluntario3 Voluntario.Nombre%TYPE DEFAULT NULL,
    IN voluntario4 Voluntario.Nombre%TYPE DEFAULT NULL,
    IN fecha_don Donacion.Fecha%TYPE DEFAULT NOW()
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Donacion Donacion.ID_Donacion%TYPE := '0';
    var_ID_Asociacion Asociacion_Ubicacion.ID_Asociacion%TYPE := '0';
BEGIN
    IF ((SELECT COUNT(*) FROM Donacion)>'0') THEN
        var_ID_Donacion:=(SELECT ID_Donacion FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1)+1;
    END IF;
    INSERT INTO Donacion VALUES (var_ID_Donacion, (SELECT ID_Producto FROM Producto WHERE Nombre=nomb_prod), (SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_negocio), (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario1), (SELECT ID_Voluntario FROM Voluntario WHERE DNI=nom_user), peso_prod, fecha_don, (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario2), (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario3), (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario4));
    IF ((SELECT COUNT(*) FROM Asociacion_Ubicacion)>'0') THEN
        var_ID_Asociacion:=(SELECT ID_Asociacion FROM Asociacion_Ubicacion ORDER BY ID_Asociacion DESC LIMIT 1)+1;
    END IF;
    INSERT INTO Asociacion_Ubicacion VALUES (var_ID_Asociacion, (SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=Numer AND ID_Tipo=(SELECT ID_Tipo FROM Tipo WHERE Nombre=puesto_o_piso)), var_ID_Asociacion, (SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_negocio));
END;
$$;

--Eliminacion de la donacion
CREATE OR REPLACE FUNCTION Eliminar_donacion
(
    IN in_fecha Donacion.Fecha%TYPE,
    IN in_negocio Negocio.Nombre%TYPE,
    IN in_producto Producto.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Donacion Donacion.ID_Donacion%type;
BEGIN
    var_ID_Donacion := (select ID_Donacion from Donacion where Fecha=in_fecha and ID_Negocio=(select ID_Negocio from Negocio where Nombre=in_negocio) and ID_Producto=(select ID_Producto from Producto where Nombre=in_producto));
    DELETE FROM Asociacion_Ubicacion WHERE ID_Donacion=var_ID_Donacion;
    DELETE FROM Donacion WHERE ID_Donacion=var_ID_Donacion;
END;
$$;

--Modificacion de la donacion
CREATE OR REPLACE FUNCTION Modificar_donacion
(
    IN nomb_prod Producto.Nombre%TYPE,
    IN nomb_negocio Negocio.Nombre%TYPE,
    IN fecha_don Donacion.Fecha%TYPE,
    IN voluntario1 Voluntario.Nombre%TYPE,
    IN peso_prod Donacion.Peso%TYPE DEFAULT NULL,
    IN voluntario2 Voluntario.Nombre%TYPE DEFAULT NULL,
    IN voluntario3 Voluntario.Nombre%TYPE DEFAULT NULL,
    IN voluntario4 Voluntario.Nombre%TYPE DEFAULT NULL
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Donacion Donacion.ID_Donacion%TYPE;
BEGIN
    var_ID_Donacion := (SELECT ID_Donacion FROM Donacion WHERE Fecha=fecha_don AND ID_Producto=(SELECT ID_Producto FROM Producto WHERE Nombre=nomb_prod) AND ID_Negocio=(SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_negocio));
    IF (peso_prod IS NOT NULL) THEN
        UPDATE Donacion SET Peso = peso_prod WHERE ID_Donacion=var_ID_Donacion;
    END IF;
    UPDATE Donacion SET ID_Voluntario1 = (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario1) WHERE ID_Donacion=var_ID_Donacion; 
    UPDATE Donacion SET ID_Voluntario2 = (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario2) WHERE ID_Donacion=var_ID_Donacion;
    UPDATE Donacion SET ID_Voluntario3 = (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario3) WHERE ID_Donacion=var_ID_Donacion;
    UPDATE Donacion SET ID_Voluntario4 = (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=voluntario4) WHERE ID_Donacion=var_ID_Donacion;
END;
$$;

--Ingresar negocio
CREATE OR REPLACE FUNCTION Ingresar_negocio
(
    IN nomb_negocio Negocio.Nombre%TYPE,
    IN mail_rec Negocio.Mail%TYPE DEFAULT NULL,
    IN tel Negocio.Telefono%TYPE DEFAULT NULL
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Negocio Negocio.ID_Negocio%TYPE := '0';
BEGIN
    IF (SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_negocio) IS NULL THEN
        IF ((SELECT COUNT(*) FROM Negocio)>'0') THEN
            nID_Negocio:=(SELECT ID_Negocio FROM Negocio ORDER BY ID_Negocio DESC LIMIT 1)+1;
        END IF;
        INSERT INTO Negocio VALUES (nID_Negocio, nomb_negocio, mail_rec, tel);
    END IF;
END;
$$;

--Modificacion de un negocio
CREATE OR REPLACE FUNCTION Modificar_negocio
(
    IN nomb_negocio Negocio.Nombre%TYPE,
    IN nuevo_nomb Negocio.Nombre%TYPE,
    IN mail_rec Negocio.Mail%TYPE DEFAULT NULL,
    IN tel Negocio.Telefono%TYPE DEFAULT NULL
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Negocio Negocio.ID_Negocio%TYPE;
BEGIN
    var_ID_Negocio := (SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_negocio);
    IF (nomb_negocio<>nuevo_nomb) THEN
        UPDATE Negocio SET Nombre = nuevo_nomb WHERE ID_Negocio=var_ID_Negocio;
    END IF;
    UPDATE Negocio SET Mail = mail_rec WHERE ID_Negocio=var_ID_Negocio;
    UPDATE Negocio SET Telefono = tel WHERE ID_Negocio=var_ID_Negocio;
END;
$$;

--Verificacion del nombre de un negocio
CREATE OR REPLACE FUNCTION Verificar_Ocupacion_Negocio
(
    IN nomb_neg      Negocio.Nombre%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT ID_Negocio FROM Negocio WHERE Nombre=nomb_neg) IS NOT NULL THEN
        RETURN TRUE; --esta ocupado
    ELSE
        RETURN FALSE; --esta libre
    END IF;
END;
$$;

--Ingresar producto
CREATE OR REPLACE FUNCTION Ingresar_producto
(
    IN nomb_prod Producto.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Producto Producto.ID_Producto%TYPE := '0';
BEGIN
    IF (SELECT ID_Producto FROM Producto WHERE Nombre=nomb_prod) IS NULL THEN
        IF ((SELECT COUNT(*) FROM Producto)>'0') THEN
            nID_Producto:=(SELECT ID_Producto FROM Producto ORDER BY ID_Producto DESC LIMIT 1)+1;
        END IF;
        INSERT INTO Producto VALUES (nID_Producto, nomb_prod);
    END IF;
END;
$$;

--Modificacion de un producto
CREATE OR REPLACE FUNCTION Modificar_producto
(
    IN nomb_prod Producto.Nombre%TYPE,
    IN nuevo_nomb Producto.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Producto Producto.ID_Producto%TYPE;
BEGIN
    var_ID_Producto := (SELECT ID_Producto FROM Producto WHERE Nombre=nomb_prod);
    IF (nomb_prod<>nuevo_nomb) THEN
        UPDATE Producto SET Nombre = nuevo_nomb WHERE ID_Producto=var_ID_Producto;
    END IF;
END;
$$;

--Verificacion del nombre de un producto
CREATE OR REPLACE FUNCTION Verificar_Ocupacion_Producto
(
    IN nomb_prod      Producto.Nombre%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT ID_Producto FROM Producto WHERE Nombre=nomb_prod) IS NOT NULL THEN
        RETURN TRUE; --esta ocupado
    ELSE
        RETURN FALSE; --esta libre
    END IF;
END;
$$;

--Ingresar duenio
CREATE OR REPLACE FUNCTION Ingresar_duenio
(
    IN nomb_duenio Duenio.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Duenio Duenio.ID_Duenio%TYPE := '0';
BEGIN
    IF (SELECT ID_Duenio FROM Duenio WHERE Nombre=nomb_duenio) IS NULL THEN
        IF ((SELECT COUNT(*) FROM Duenio)>'0') THEN
            nID_Duenio:=(SELECT ID_Duenio FROM Duenio ORDER BY ID_Duenio DESC LIMIT 1)+1;
        END IF;
        INSERT INTO Duenio VALUES (nID_Duenio, nomb_duenio);
    END IF;
END;
$$;

--Modificacion de un dueño
CREATE OR REPLACE FUNCTION Modificar_duenio
(
    IN nomb_duenio Duenio.Nombre%TYPE,
    IN nuevo_nomb Duenio.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Duenio Duenio.ID_Duenio%TYPE;
BEGIN
    var_ID_Duenio := (SELECT ID_Duenio FROM Duenio WHERE Nombre=nomb_duenio);
    IF (nomb_duenio<>nuevo_nomb) THEN
        UPDATE Duenio SET Nombre = nuevo_nomb WHERE ID_Duenio=var_ID_Duenio;
    END IF;
END;
$$;

--Verificacion del nombre de un dueño
CREATE OR REPLACE FUNCTION Verificar_Ocupacion_Duenio
(
    IN nomb_due      Duenio.Nombre%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT ID_Duenio FROM Duenio WHERE Nombre=nomb_due) IS NOT NULL THEN
        RETURN TRUE; --esta ocupado
    ELSE
        RETURN FALSE; --esta libre
    END IF;
END;
$$;

--Ingresar voluntario
CREATE OR REPLACE FUNCTION Ingresar_voluntario
(
    IN nomb_vol     Voluntario.Nombre%TYPE,
    IN dni_vol      Voluntario.DNI%TYPE,
    IN admin        Voluntario.Es_Admin%TYPE,
    IN pass         Voluntario.Pass%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Voluntario Voluntario.ID_Voluntario%TYPE := '0';
BEGIN
    IF (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=nomb_vol) IS NULL THEN
        IF ((SELECT COUNT(*) FROM Voluntario)>'0') THEN
            nID_Voluntario:=(SELECT ID_Voluntario FROM Voluntario ORDER BY ID_Voluntario DESC LIMIT 1)+1;
        END IF;
        INSERT INTO Voluntario VALUES (nID_Voluntario, nomb_vol,dni_vol,admin,PGP_SYM_ENCRYPT(pass,'AES_KEY'));
    END IF;
END;
$$;

--Modificacion de un voluntario
CREATE OR REPLACE FUNCTION Modificar_voluntario
(
    IN nuevo_nom  Voluntario.Nombre%TYPE,
    IN DNI_user   Voluntario.Nombre%TYPE,
    IN pass_user  Voluntario.Pass%TYPE,
    IN npass_user Voluntario.Pass%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Voluntario Voluntario.ID_Voluntario%TYPE;
BEGIN
    var_ID_Voluntario := (SELECT ID_Voluntario FROM Voluntario WHERE DNI=DNI_user);
    IF (pass_user=(SELECT pgp_sym_decrypt(Pass::bytea,'AES_KEY') FROM Voluntario WHERE ID_Voluntario=var_ID_Voluntario)) THEN
            IF ((SELECT Nombre FROM Voluntario WHERE ID_Voluntario=var_ID_Voluntario)<>nuevo_nom) THEN --Actualizo el nombre
                UPDATE Voluntario SET Nombre = nuevo_nom WHERE ID_Voluntario=var_ID_Voluntario;
            END IF;
            IF npass_user IS NOT NULL THEN
                UPDATE Voluntario SET Pass = PGP_SYM_ENCRYPT(npass_user,'AES_KEY') WHERE ID_Voluntario=var_ID_Voluntario; --Actualizo la pass
            END IF;
    END IF;
END;
$$;

--Verificacion del nombre de un voluntario
CREATE OR REPLACE FUNCTION Verificar_Ocupacion_Voluntario_nombre
(
    IN Nom_vol      Voluntario.Nombre%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT ID_Voluntario FROM Voluntario WHERE Nombre=Nom_vol) IS NOT NULL THEN
        RETURN TRUE; --esta ocupado
    ELSE
        RETURN FALSE; --esta libre
    END IF;
END;
$$;

--Verificacion del dni de un voluntario
CREATE OR REPLACE FUNCTION Verificar_Ocupacion_Voluntario_dni
(
    IN DNI_vol      Voluntario.DNI%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT ID_Voluntario FROM Voluntario WHERE DNI=DNI_vol) IS NOT NULL THEN
        RETURN TRUE; --esta ocupado
    ELSE
        RETURN FALSE; --esta libre
    END IF;
END;
$$;

--Ingresar ubicacion
CREATE OR REPLACE FUNCTION Ingresar_ubicacion
(
    IN numer Ubicacion.Numero%TYPE,
    IN p_o_p Tipo.Nombre%TYPE,
    IN duen  Duenio.Nombre%TYPE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Ubicacion Ubicacion.ID_Ubicacion%TYPE := '0';
    npop          Tipo.ID_Tipo%TYPE;
BEGIN
    IF (p_o_p = 'piso') THEN
        npop := 1;
    ELSE
        npop := 2;
    END IF;
    IF (SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=numer AND ID_Tipo=npop) IS NULL THEN
        IF ((SELECT COUNT(*) FROM Ubicacion)>'0') THEN
            nID_Ubicacion:=(SELECT ID_Ubicacion FROM Ubicacion ORDER BY ID_Ubicacion DESC LIMIT 1)+1;
        END IF;
        INSERT INTO Ubicacion VALUES (nID_Ubicacion, npop, numer, (SELECT ID_Duenio FROM Duenio WHERE Nombre=duen));
    END IF;
END;
$$;

--Verificar ubicacion
CREATE OR REPLACE FUNCTION Verificar_ubicacion
(
    IN numer Ubicacion.Numero%TYPE,
    IN p_o_p Tipo.Nombre%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
DECLARE
    nID_Ubicacion Ubicacion.ID_Ubicacion%TYPE := '0';
    npop          Tipo.ID_Tipo%TYPE;
BEGIN
    IF (p_o_p = 'piso') THEN
        npop := 1;
    ELSE
        npop := 2;
    END IF;
    IF (SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=numer AND ID_Tipo=npop) IS NOT NULL THEN
        RETURN TRUE; -- esta ocupado
    ELSE
        RETURN FALSE; -- esta libre
    END IF;
END;
$$;

--Modificacion de una ubicacion
CREATE OR REPLACE FUNCTION Modificar_ubicacion
(
    IN numer Ubicacion.Numero%TYPE,
    IN p_o_p Tipo.Nombre%TYPE,
    IN duen  Duenio.ID_Duenio%TYPE DEFAULT NULL
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    var_ID_Ubicacion Ubicacion.ID_Ubicacion%TYPE;
    npop             Tipo.ID_Tipo%TYPE;
BEGIN
    IF (p_o_p = 'piso') THEN
        npop := 1;
    ELSE
        npop := 2;
    END IF;
    var_ID_Ubicacion := (SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=numer AND ID_Tipo=npop);
    UPDATE Ubicacion SET ID_Duenio=duen WHERE ID_Ubicacion=var_ID_Ubicacion;
END;
$$;

--Validacion del usuario
CREATE OR REPLACE FUNCTION Verificar_Usuario
(
    IN nomb_us      Voluntario.Nombre%TYPE,
    IN pass_us      Voluntario.Pass%TYPE
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
BEGIN
    IF (SELECT COUNT(*) FROM Voluntario WHERE DNI=nomb_us) = 1 AND (SELECT pgp_sym_decrypt(Pass::bytea,'AES_KEY') FROM Voluntario WHERE DNI=nomb_us)=pass_us THEN
        RETURN TRUE;
    ELSE
        RETURN FALSE;
    END IF;
END;
$$;

--Retornar Tabla Donaciones
CREATE OR REPLACE FUNCTION obtener_tabla()
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    indice_fila CURSOR FOR SELECT ID_Donacion FROM Donacion ORDER BY ID_Donacion DESC;
    cur_ID_Donacion Donacion.ID_Donacion%type;
    var_Fecha       DATE;
    var_Vol1        VARCHAR DEFAULT NULL;
    var_Vol2        VARCHAR DEFAULT NULL;
    var_Vol3        VARCHAR DEFAULT NULL;
    var_Vol4        VARCHAR DEFAULT NULL;
    var_nomb_neg    VARCHAR DEFAULT NULL;
    var_numero      INTEGER;
    var_P_o_P       VARCHAR DEFAULT NULL;
    var_telefono    VARCHAR DEFAULT NULL;
    var_mail        VARCHAR DEFAULT NULL;
    var_duenio      VARCHAR DEFAULT NULL;
    var_producto    VARCHAR DEFAULT NULL;
    var_peso        REAL;
BEGIN
    TRUNCATE TABLE auxiliar;
    OPEN indice_fila;
    LOOP
    FETCH NEXT FROM indice_fila INTO cur_ID_Donacion;
	EXIT WHEN NOT FOUND;
        var_Fecha:=(SELECT Fecha FROM Donacion WHERE ID_Donacion=cur_ID_Donacion);
        var_Vol1:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario1 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF ((SELECT ID_Voluntario2 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol2:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario2 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol2:='Sin Completar';
        END IF;
        IF ((SELECT ID_Voluntario3 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol3:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario3 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol3:='Sin Completar';
        END IF;
        IF ((SELECT ID_Voluntario4 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol4:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario4 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol4:='Sin Completar';
        END IF;
        var_nomb_neg:=(SELECT Nombre FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        var_numero := (SELECT Numero FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion));
        var_P_o_P := (SELECT Nombre FROM Tipo WHERE ID_Tipo=(SELECT ID_Tipo FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion)));
        var_telefono:=(SELECT Telefono FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF (var_telefono IS NULL) THEN
            var_telefono:='Sin Completar';
        END IF;
        var_mail:=(SELECT Mail FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF (var_mail IS NULL) THEN
            var_mail:='Sin Completar';
        END IF;
        var_duenio := (SELECT Nombre FROM Duenio WHERE ID_Duenio=(SELECT ID_Duenio FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion)));
        IF (var_duenio IS NULL) THEN
            var_duenio:='Sin Completar';
        END IF;
        var_producto:=(SELECT Nombre FROM Producto WHERE ID_Producto=(SELECT ID_Producto FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        var_peso:=(SELECT Peso FROM Donacion WHERE ID_Donacion=cur_ID_Donacion);
        INSERT INTO Auxiliar VALUES (var_Fecha,var_Vol1,var_Vol2,var_Vol3,var_Vol4,var_nomb_neg,var_numero,var_P_o_P,var_telefono,var_mail,var_duenio,var_producto,var_peso);
    END LOOP;
    CLOSE indice_fila;
END;
$$;

--Retornar Tabla Donaciones filtrada
CREATE OR REPLACE FUNCTION obtener_tabla_filtrada
(
    IN fecha_selec DATE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    indice_fila CURSOR FOR SELECT ID_Donacion FROM Donacion WHERE Fecha=fecha_selec ORDER BY ID_Donacion;
    cur_ID_Donacion Donacion.ID_Donacion%type;
    var_Fecha       DATE;
    var_Vol1        VARCHAR DEFAULT NULL;
    var_Vol2        VARCHAR DEFAULT NULL;
    var_Vol3        VARCHAR DEFAULT NULL;
    var_Vol4        VARCHAR DEFAULT NULL;
    var_nomb_neg    VARCHAR DEFAULT NULL;
    var_numero      INTEGER;
    var_P_o_P       VARCHAR DEFAULT NULL;
    var_telefono    VARCHAR DEFAULT NULL;
    var_mail        VARCHAR DEFAULT NULL;
    var_duenio      VARCHAR DEFAULT NULL;
    var_producto    VARCHAR DEFAULT NULL;
    var_peso        REAL;
BEGIN
    TRUNCATE TABLE auxiliar;
    OPEN indice_fila;
    LOOP
    FETCH NEXT FROM indice_fila INTO cur_ID_Donacion;
	EXIT WHEN NOT FOUND;
        var_Fecha:=(SELECT Fecha FROM Donacion WHERE ID_Donacion=cur_ID_Donacion);
        var_Vol1:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario1 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF ((SELECT ID_Voluntario2 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol2:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario2 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol2:='Sin Completar';
        END IF;
        IF ((SELECT ID_Voluntario3 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol3:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario3 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol3:='Sin Completar';
        END IF;
        IF ((SELECT ID_Voluntario4 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion) IS NOT NULL) THEN
            var_Vol4:=(SELECT Nombre FROM Voluntario WHERE ID_Voluntario=(SELECT ID_Voluntario4 FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        ELSE
            var_Vol4:='Sin Completar';
        END IF;
        var_nomb_neg:=(SELECT Nombre FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        var_numero := (SELECT Numero FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion));
        var_P_o_P := (SELECT Nombre FROM Tipo WHERE ID_Tipo=(SELECT ID_Tipo FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion)));
        var_telefono:=(SELECT Telefono FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF (var_telefono IS NULL) THEN
            var_telefono:='Sin Completar';
        END IF;
        var_mail:=(SELECT Mail FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        IF (var_mail IS NULL) THEN
            var_mail:='Sin Completar';
        END IF;
        var_duenio := (SELECT Nombre FROM Duenio WHERE ID_Duenio=(SELECT ID_Duenio FROM Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion=cur_ID_Donacion)));
        IF (var_duenio IS NULL) THEN
            var_duenio:='Sin Completar';
        END IF;
        var_producto:=(SELECT Nombre FROM Producto WHERE ID_Producto=(SELECT ID_Producto FROM Donacion WHERE ID_Donacion=cur_ID_Donacion));
        var_peso:=(SELECT Peso FROM Donacion WHERE ID_Donacion=cur_ID_Donacion);
        INSERT INTO Auxiliar VALUES (var_Fecha,var_Vol1,var_Vol2,var_Vol3,var_Vol4,var_nomb_neg,var_numero,var_P_o_P,var_telefono,var_mail,var_duenio,var_producto,var_peso);
    END LOOP;
    CLOSE indice_fila;
END;
$$;

--------------------Funciones Resumen--------------------
--Muestra el total en kg de cada producto en un dia especifico
CREATE OR REPLACE FUNCTION resumen_peso_producto_diario 
(
    IN fecha_resum DATE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    indice_prod CURSOR FOR SELECT P.Nombre, SUM(D.Peso) FROM Donacion D INNER JOIN Producto P ON D.ID_Producto=P.ID_Producto WHERE D.Fecha=fecha_resum GROUP BY P.Nombre;
    cur_Prod Producto.Nombre%type;
	cur_Peso Donacion.Peso%type;
BEGIN
    TRUNCATE TABLE Auxiliar;
    OPEN indice_prod;
    LOOP
        FETCH NEXT FROM indice_prod INTO cur_Prod, cur_Peso;
	    EXIT WHEN NOT FOUND;
        INSERT INTO Auxiliar (producto, peso) VALUES (cur_Prod, cur_Peso); 
    END LOOP;
    CLOSE indice_prod;
END;
$$;

--Muestra el total en kg obtenidos en un dia especifico
CREATE OR REPLACE FUNCTION resumen_peso_total_diario 
(
    IN fecha_resum DATE
)
RETURNS REAL
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN (SELECT SUM(Peso) FROM Donacion WHERE Fecha=fecha_resum);
END;
$$;

--Muestra el total en kg de cada donante por semestre
CREATE OR REPLACE FUNCTION resumen_peso_total_donante_semestre
(
    IN fecha_resum DATE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    indice_don_sem1 CURSOR FOR SELECT SUM(D.peso), N.Nombre FROM donacion D INNER JOIN negocio N ON D.ID_Negocio=N.ID_Negocio WHERE EXTRACT(YEAR FROM D.Fecha)=EXTRACT(YEAR FROM fecha_resum) AND EXTRACT(MONTH FROM D.Fecha) BETWEEN 1 AND 6 GROUP BY N.Nombre;
    indice_don_sem2 CURSOR FOR SELECT SUM(D.peso), N.Nombre FROM donacion D INNER JOIN negocio N ON D.ID_Negocio=N.ID_Negocio WHERE EXTRACT(YEAR FROM D.Fecha)=EXTRACT(YEAR FROM fecha_resum) AND EXTRACT(MONTH FROM D.Fecha) BETWEEN 7 AND 12 GROUP BY N.Nombre;
    cur_Don Negocio.Nombre%type;
	cur_Peso Donacion.Peso%type;
    mes  INT := CAST(EXTRACT(MONTH FROM fecha_resum) AS INT);
BEGIN
    TRUNCATE TABLE Auxiliar;
    IF (mes BETWEEN 1 AND 6) THEN
        OPEN indice_don_sem1;
        LOOP
            FETCH NEXT FROM indice_don_sem1 INTO cur_Peso, cur_Don;
	        EXIT WHEN NOT FOUND;
            INSERT INTO Auxiliar (nomb_neg, peso) VALUES (cur_Don, cur_Peso); 
        END LOOP;
        CLOSE indice_don_sem1;
    ELSE
        OPEN indice_don_sem2;
        LOOP
            FETCH NEXT FROM indice_don_sem2 INTO cur_Peso, cur_Don;
	        EXIT WHEN NOT FOUND;
            INSERT INTO Auxiliar (nomb_neg, peso) VALUES (cur_Don, cur_Peso); 
        END LOOP;
        CLOSE indice_don_sem2;
    END IF;
END;
$$;

--Muestra el total en kg de cada donante por año
CREATE OR REPLACE FUNCTION resumen_peso_total_donante_anio
(
    IN fecha_resum DATE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    indice_don_anual CURSOR FOR SELECT SUM(D.peso), N.Nombre FROM donacion D INNER JOIN negocio N ON D.ID_Negocio=N.ID_Negocio WHERE EXTRACT(YEAR FROM D.Fecha)=EXTRACT(YEAR FROM fecha_resum) GROUP BY N.Nombre;
    cur_Don Negocio.Nombre%type;
	cur_Peso Donacion.Peso%type;
BEGIN
    TRUNCATE TABLE Auxiliar;
    OPEN indice_don_anual;
    LOOP
        FETCH NEXT FROM indice_don_anual INTO cur_Peso, cur_Don;
	    EXIT WHEN NOT FOUND;
        INSERT INTO Auxiliar (nomb_neg, peso) VALUES (cur_Don, cur_Peso); 
    END LOOP;
    CLOSE indice_don_anual;
END;
$$;

--Suma las ventas de cada mes y las divide por la cantidad de donaciones dicho mes y realiza eso para todos los meses
CREATE OR REPLACE FUNCTION resumen_promedio_peso_total_mes
(
    IN fecha_resum DATE
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    mes INT := '1';
    anio INT :=CAST(EXTRACT(YEAR FROM fecha_resum) AS INT);
    cant_dias INT;
    total_mes REAL;
    promedio REAL;
BEGIN
    TRUNCATE TABLE Auxiliar;
    LOOP
    EXIT WHEN mes='13';
        cant_dias:= CAST ((SELECT COUNT(*) FROM Donacion WHERE CAST(EXTRACT(MONTH FROM Fecha) AS INT)=mes and CAST(EXTRACT(YEAR FROM Fecha) AS INT)=anio) AS INT);
        total_mes:= CAST ((SELECT SUM(peso) FROM Donacion WHERE CAST(EXTRACT(MONTH FROM Fecha) AS INT)=mes and CAST(EXTRACT(YEAR FROM Fecha) AS INT)=anio) AS REAL);
        promedio:= total_mes/cant_dias;
        INSERT INTO Auxiliar (peso) VALUES (promedio);
        mes := mes + '1';
    END LOOP;
END;
$$;

--------------------Recomendaciones--------------------
--(esto se da cuando se abre la ventana) Se devuelven el ultimo grupo de voluntarios y el ultimo negocio utilizado
CREATE OR REPLACE FUNCTION recom_ult_don()
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    idvol1 Voluntario.ID_Voluntario%TYPE := 0;
    idvol2 Voluntario.ID_Voluntario%TYPE := 0;
    idvol3 Voluntario.ID_Voluntario%TYPE := 0;
    idvol4 Voluntario.ID_Voluntario%TYPE := 0;
    id_neg Negocio.ID_Negocio%TYPE := 0;
    --id_ubic Ubicacion.ID_Ubicacion%TYPE := 0;
BEGIN
    TRUNCATE TABLE Auxiliar;
    idvol1 := (SELECT ID_Voluntario FROM Voluntario WHERE ID_Voluntario = (SELECT ID_Voluntario1 FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1));
    idvol2 := (SELECT ID_Voluntario FROM Voluntario WHERE ID_Voluntario = (SELECT ID_Voluntario2 FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1));
    idvol3 := (SELECT ID_Voluntario FROM Voluntario WHERE ID_Voluntario = (SELECT ID_Voluntario3 FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1));
    idvol4 := (SELECT ID_Voluntario FROM Voluntario WHERE ID_Voluntario = (SELECT ID_Voluntario4 FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1));
    id_neg := (SELECT ID_Negocio FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1);
    --id_ubic := (SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Donacion = (SELECT ID_Donacion FROM Donacion ORDER BY ID_Donacion DESC LIMIT 1));
    INSERT INTO Auxiliar(vol1, vol2, vol3, vol4, nomb_neg/*, numero, P_o_P*/) VALUES (idvol1, idvol2, idvol3, idvol4, id_neg/*, (SELECT Numero FROM Ubicacion WHERE ID_Ubicacion=id_ubic), (SELECT Nombre FROM Tipo WHERE ID_Tipo = (SELECT ID_Tipo FROM Ubicacion WHERE ID_Ubicacion=id_ubic))*/);
END;
$$;

--------------------Auxiliares--------------------
--Permite autocompletar la info de un puesto si este esta ocupado
CREATE OR REPLACE FUNCTION obtener_info_puesto
(
    IN num_recib INTEGER
)
RETURNS VOID
LANGUAGE plpgsql
AS
$$
DECLARE
    ID_neg Negocio.ID_Negocio%TYPE;
BEGIN
    TRUNCATE TABLE Auxiliar_ID;
    ID_neg := (SELECT ID_Negocio FROM Asociacion_Ubicacion WHERE ID_Ubicacion = (SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=num_recib AND ID_Tipo='2') LIMIT 1);
    INSERT INTO Auxiliar_ID VALUES (ID_neg);
END;
$$;

--Permite verificar que no se agregue un puesto con un numero nuevo si este ya esta ocupado
CREATE OR REPLACE FUNCTION verificar_puesto
(
    IN num_recib INTEGER,
    IN nomb_neg  VARCHAR
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
DECLARE
    ocupacion BOOLEAN DEFAULT FALSE; --Falso significa que esta libre
	id_ubi    Ubicacion.ID_Ubicacion%TYPE := 0;
BEGIN
	id_ubi := (SELECT ID_Ubicacion FROM Asociacion_Ubicacion WHERE ID_Ubicacion=(SELECT ID_Ubicacion FROM Ubicacion WHERE Numero=num_recib AND ID_Tipo=2 LIMIT 1) LIMIT 1);
    IF id_ubi IS NULL THEN
        RETURN FALSE;
    ELSE
        IF nomb_neg = (SELECT Nombre FROM Negocio WHERE ID_Negocio=(SELECT ID_Negocio FROM Asociacion_Ubicacion WHERE ID_Ubicacion=id_ubi LIMIT 1)) THEN
			RETURN FALSE;
		ELSE
			RETURN TRUE;
		END IF;
    END IF;
END;
$$;

--Permite verificar si exite un usuario administrador en la BD
CREATE OR REPLACE FUNCTION verificar_users_admin()
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
DECLARE
BEGIN
    IF (SELECT COUNT(*) FROM Voluntario WHERE Es_Admin=TRUE) >= 1 THEN
        RETURN TRUE; --existen usuarios
    ELSE
        RETURN FALSE; --no existen usuarios ADMIN
    END IF;
END;
$$;

--Verifica que no se pueda ingresar dos veces en un dia la relacion negocio-producto-fecha (un mismo negocio en un mismo dia no deberia registrar dos donaciones de un producto, sino una sola donde este el total)
CREATE OR REPLACE FUNCTION verificar_unicidad --Falso significa que ya existe una, por lo que no debe dejar continuar
(
    IN fecha_recib DATE,
    IN nomb_neg    VARCHAR,
    IN nom_prod    VARCHAR
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS
$$
DECLARE
    id_don Donacion.ID_Donacion%TYPE DEFAULT NULL;
BEGIN
    id_don := (select ID_Donacion FROM Donacion WHERE Fecha=fecha_recib AND ID_Producto=(SELECT ID_Producto FROM Producto WHERE Nombre=nom_prod) AND ID_Negocio=(select ID_Negocio FROM Negocio WHERE Nombre=nomb_neg));
    IF id_don IS NOT NULL THEN
        RETURN FALSE;
    ELSE
        RETURN TRUE;
    END IF;
END;
$$;