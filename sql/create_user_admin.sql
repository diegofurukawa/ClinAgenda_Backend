-- Inserir um usuário básico para testes
INSERT INTO `user` (
    `username`, 
    `email`, 
    `passwordHash`, 
    `dLastLogin`, 
    `nFailedLoginAttempts`, 
    `dCreated`, 
    `dLastUpdated`, 
    `lActive`
) 
VALUES (
    'admin', 
    'admin@clinagenda.com', 
    'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', -- senha: admin123 (utilizando bcrypt)
    NULL, 
    0, 
    NOW(), 
    NOW(), 
    1
);

-- Inserir um perfil básico de administrador
INSERT INTO `role` (
    `roleName`, 
    `description`, 
    `dCreated`, 
    `dLastUpdated`, 
    `lActive`
) 
VALUES (
    'admin', 
    'Administrador do sistema com acesso total', 
    NOW(), 
    NOW(), 
    1
);

-- Associar o usuário ao perfil de administrador
INSERT INTO `user_role` (
    `userId`, 
    `roleId`, 
    `dCreated`, 
    `dLastUpdated`, 
    `lActive`
) 
VALUES (
    1, -- assumindo que o usuário admin terá ID 1
    1, -- assumindo que o perfil admin terá ID 1
    NOW(), 
    NOW(), 
    1
);