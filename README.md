ТЗ прикрепил с проектом.

Согласно ТЗ выполнил следующие требования к реализации:
- Стек: C# (net 6), ASP.NET, ADO.NET для работы с БД, JavaScript, HTML5, CSS3
- Структурировал проект
- Использовал Yandex maps API
- Пароли пользователей не хранятся в явном виде. К паролю добавил соль и хэшировал строку с помощью sha-256. При аутентификации сверяются именно хэши.
- JWT
- REST API
- В качестве базы данных использовал PostgreSQL версии 14
- Swagger

Что хотел бы исправить:
- JWT. Реализовать стандарт строго по RFC
- Взаимодействие с БД. Выполнить работу с помощью EF и реализовать пул соединений

Как создать БД:
````
create database kompasdb;

create user kompasuser with encrypted password 'kompasuser';

grant all privileges on database kompasdb to kompasuser;

\c kompasdb

/***************/
/* table users */
/***************/
create table users
(
    user_id         uuid            PRIMARY KEY     DEFAULT gen_random_uuid(),
    user_login      varchar(20)     UNIQUE          NOT NULL,
    password_hash   character(64)                   NOT NULL,
    salt            bytea           UNIQUE          NOT NULL
);

insert into users(user_login, password_hash, salt)
values ('denis', /* password is qwerty */
        '75a531c0c68110d8ff171f159dc602494c772efeb13c77d736cd94f655e0be39', 
        decode('25FC5D18FB7723B63E24872BE03FDC0C', 'hex'));

insert into users(user_login, password_hash, salt)
values ('anatoly0', /* password is Kompas */
        '1dd35c7b29d2dd99f4928c887663b2de72e3460681e728012d354de31bab8a23', 
        decode('C04775A7D19E85A35F3D69FA91DE6282', 'hex'));

insert into users(user_login, password_hash, salt)
values ('vasya', /* password is simplePassword */
        'be0ca70a8ee315d031ca8a6221e53e35ba53de11149a185a12ceeecbd84e621d', 
        decode('9ABED59BDC97D0B5EF807FC997B8BE34', 'hex'));

insert into users(user_login, password_hash, salt)
values ('some0ne', /* password is passF0rUser */
        '60f2a8f03351e2a8a52ee73af52584010856ca08e6cc297a0e2697bb4ef4ee8d', 
        decode('214B4C7A1FB4533D05252D9BB890916C', 'hex'));

/***********/
/* markers */
/***********/
create table markers 
(
    marker_id   serial          PRIMARY KEY,
    marker_text varchar(128)    NOT NULL,
    latitude    varchar(40)     NOT NULL,
    longitude   varchar(40)     NOT NULL
);

create index marker_latitude_index on markers(latitude);

create index marker_longitude_index on markers(longitude);

insert into markers(marker_text, latitude, longitude)
values ('Серебряный бор', '55.782469', '37.431951');

insert into markers(marker_text, latitude, longitude)
values ('Отрадное', '55.865623', '37.606498');

insert into markers(marker_text, latitude, longitude)
values ('Шаболовская. У компании Компас здесь будет новый офис', '55.718884', '37.608099');

insert into markers(marker_text, latitude, longitude)
values ('Белорусская. Текущий офис Компании Компас', '55.781554', '37.570187');


/******************/
/* refresh_tokens */
/******************/
create table refresh_tokens
(
    username        varchar(20) NOT NULL,
    refresh_token   varchar(36) NOT NULL,
    valid_until     varchar(64) NOT NULL
);

create index refresh_tokens_index on refresh_tokens(refresh_token);
````
