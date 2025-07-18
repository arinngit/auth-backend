CREATE OR REPLACE FUNCTION fn_AddUser(email varchar, passwordHash varchar, salt varchar, isEmailConfirmed boolean, roleId integer, profileImageUrl varchar)
RETURNS "Users" AS $$
DECLARE
	result "Users";
BEGIN
	INSERT INTO "Users" ("Email", "PasswordHash", "Salt", "IsEmailConfirmed", "RoleId", "ProfileImageUrl")
	VALUES (email, passwordHash, salt, isEmailConfirmed, roleId, profileImageUrl)
	RETURNING * INTO result;

	RETURN result;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION func_AddRefreshToken(p_userId integer, p_token text, p_expires timestamp with time zone)
RETURNS "RefreshTokens" AS $$
DECLARE
    result "RefreshTokens";
BEGIN
    INSERT INTO "RefreshTokens" ("Token", "ExpiresAt", "UserId")
    VALUES (p_token, p_expires, p_userId)
    RETURNING * INTO result;

    RETURN result;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION func_GetRefreshTokenByToken(oldToken text)
    RETURNS SETOF "RefreshTokens" AS
$$
BEGIN
    RETURN QUERY
        SELECT *
        FROM "RefreshTokens"
        WHERE "Token" = oldToken;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION func_RemoveOldRefreshToken(p_token varchar)
    RETURNS boolean AS
$$
BEGIN
    DELETE FROM "RefreshTokens"
    WHERE "Token" = p_token;

    IF FOUND THEN
        RETURN true;
    END IF;

    RETURN false;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION func_RemoveUserAllRefreshTokens(p_userId integer)
    RETURNS boolean AS
$$
DECLARE
    rows_affected integer;
BEGIN
    DELETE
    FROM "RefreshTokens"
    WHERE "UserId" = p_userId;

    GET DIAGNOSTICS rows_affected = ROW_COUNT;

    RETURN rows_affected > 0;
END;
$$ LANGUAGE plpgsql;
