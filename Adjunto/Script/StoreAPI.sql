-- Creando las tablas necesarias para la API de la tienda

-- Tabla de Roles
CREATE TABLE [dbo].[Rol](
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [name] NVARCHAR(50) NOT NULL
);

-- Insertando roles iniciales
INSERT INTO [dbo].[Rol] ([name]) VALUES ('User');
INSERT INTO [dbo].[Rol] ([name]) VALUES ('Admin');

-- Tabla de Usuarios
CREATE TABLE [dbo].[User](
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [name] NVARCHAR(100) NOT NULL,
    [email] NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(200) NOT NULL, -- Guarda hash
    [created_on_utc] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
    [rol] INT NOT NULL DEFAULT 1, -- 1 = User, 2 = Admin
    FOREIGN KEY ([rol]) REFERENCES [Rol]([id])
);
CREATE UNIQUE INDEX IX_User_Email ON [dbo].[User]([email]);

-- Usuario provisional para pruebas
INSERT INTO [User] ([name], [email], [password], [created_on_utc], [rol])
VALUES (
    'Test User',                                                    -- Name
    'user@gmail.com',                                               -- Email
    '$2a$10$rXxlG.hblwW6rx6vlvwD3.kUEX6RzY4p8aMAUyik9tJCjYm5aOdMq', -- PasswordHash
    SYSUTCDATETIME(),                                               -- CreatedOnUtc
    1                                                               -- RolId (1 = User)
);
GO

INSERT INTO [User] ([name], [email], [password], [created_on_utc], [rol])
VALUES (
    'Test User Admin',                                              -- Name
    'admin@gmail.com',                                              -- Email
    '$2a$10$rXxlG.hblwW6rx6vlvwD3.kUEX6RzY4p8aMAUyik9tJCjYm5aOdMq', -- PasswordHash
    SYSUTCDATETIME(),                                               -- CreatedOnUtc
    2                                                               -- RolId (2 = Admin)
);
GO


-- Tabla Productos
CREATE TABLE [dbo].[Product](
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [name] NVARCHAR(100) NOT NULL,
    [description] NVARCHAR(MAX) NULL,
    [stock] DECIMAL(20,8) NOT NULL,
    [updated_stock_on_utc] DATETIME NULL,
    [unit_price] DECIMAL(20,8) NOT NULL,
    [created_on_utc] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
    [is_deleted] BIT NOT NULL DEFAULT 0, -- No eliminado por defecto
    [deleted_on_utc] DATETIME NULL
);

-- Historial de cambios de precio
CREATE TABLE [dbo].[Product_price_log](
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [old_price] DECIMAL(20,8) NOT NULL,
    [new_price] DECIMAL(20,8) NOT NULL,
    [changed_on_utc] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
    [changed_by] INT NOT NULL,
    [product_id] INT NOT NULL,
    FOREIGN KEY ([changed_by]) REFERENCES [User]([id]),
    FOREIGN KEY ([product_id]) REFERENCES [Product]([id])
);

-- Log de compras
CREATE TABLE [dbo].[Purchase](
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [quantity] DECIMAL(20,8) NOT NULL,
    [total_amount] DECIMAL(20,8) NOT NULL,
    [purchased_on_utc] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
    [product_id] INT NOT NULL,
    [purchased_by] INT NOT NULL,
    FOREIGN KEY ([product_id]) REFERENCES [Product]([id]),
    FOREIGN KEY ([purchased_by]) REFERENCES [User]([id])
);


--------------------------------------------------------------------------------
-- Apartado para procedimientos almacenados

-- Procedimiento para agregar un nuevo producto
CREATE PROCEDURE sp_add_product
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Stock DECIMAL(20,8),
    @UnitPrice DECIMAL(20,8)

AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO Product (name, description, stock, unit_price)
        VALUES (@Name, @Description, @Stock, @UnitPrice);

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO



-- Procedimiento para obtener todos los productos no eliminados
CREATE PROCEDURE sp_get_all_products_name_asc
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            id,
            name,
            description,
            stock,
            updated_stock_on_utc,
            unit_price,
            created_on_utc,
            is_deleted,
            deleted_on_utc
        FROM Product
        WHERE is_deleted = 0
        ORDER BY name ASC;

    END TRY

    BEGIN CATCH
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO

-- Procedimiento para obtener productos por nombre (busqueda parcial)
CREATE PROCEDURE sp_get_products_by_name
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            id,
            name,
            description,
            stock,
            updated_stock_on_utc,
            unit_price,
            created_on_utc,
            is_deleted,
            deleted_on_utc
        FROM Product
        WHERE is_deleted = 0
          AND name LIKE '%' + @name + '%'
        ORDER BY name ASC;

    END TRY

    BEGIN CATCH
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH
END
GO

-- Procedimiento para obtener un producto por su ID
CREATE PROCEDURE sp_get_product_by_id
    @Id INT
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            id,
            name,
            description,
            stock,
            updated_stock_on_utc,
            unit_price,
            created_on_utc,
            is_deleted,
            deleted_on_utc
        FROM Product
        WHERE id = @Id
        AND is_deleted = 0; -- Solo productos activos

        END TRY

    BEGIN CATCH
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO


-- Procedimiento para eliminar un producto (soft delete)
CREATE PROCEDURE sp_soft_delete_product
    @Id INT
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Product
        SET is_deleted = 1,
            deleted_on_utc = SYSUTCDATETIME()
        WHERE id = @Id;

    END TRY

    BEGIN CATCH
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO


-- Procedimiento para actualizar el stock de un producto
CREATE PROCEDURE sp_update_product_stock
    @Id INT,
    @NewStock DECIMAL(20,8)
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Product
        SET stock = @NewStock,
            updated_stock_on_utc = SYSUTCDATETIME()
        WHERE Id = @Id;
    END TRY

    BEGIN CATCH
        
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO


-- Procedimiento para actualizar el precio de un producto y registrar el cambio
CREATE PROCEDURE sp_update_product_price
    @ProductId INT,
    @OldPrice DECIMAL(20,8),
    @NewPrice DECIMAL(20,8),
    @ChangedBy INT -- Id del usuario que realiza el cambio
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar en el log de cambios de precio usando el old_price enviado desde el service
        INSERT INTO Product_price_log (product_id, old_price, new_price, changed_on_utc, changed_by)
        VALUES (@ProductId, @OldPrice, @NewPrice, SYSUTCDATETIME(), @ChangedBy);

        -- Actualizar el precio del producto
        UPDATE Product
        SET unit_price = @NewPrice
        WHERE Id = @ProductId;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO



CREATE PROCEDURE sp_purchase_product
    @ProductId INT,
    @Quantity DECIMAL(20,8),
    @UserId INT,
    @TotalAmount DECIMAL(20,8),
    @NewStock DECIMAL(20,8)
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Actualizar el stock del producto
        UPDATE Product
        SET stock = @NewStock,
            updated_stock_on_utc = SYSUTCDATETIME()
        WHERE Id = @ProductId;

        -- Insertar la compra
        INSERT INTO Purchase (quantity, total_amount, purchased_on_utc, product_id, purchased_by)
        VALUES (@Quantity, @TotalAmount, SYSUTCDATETIME(), @ProductId, @UserId);

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO


-- Procedimiento para obtener un usuario por su email
CREATE PROCEDURE sp_get_user_by_email
    @Email NVARCHAR(100)
AS BEGIN SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            id,
            name,
            email,
            password,
            created_on_utc,
            rol
        FROM [User]
        WHERE email = @Email

        RETURN 1; -- exito
    END TRY

    BEGIN CATCH
        THROW; -- Lanzar el error para que EF lo capture
    END CATCH

END
GO
