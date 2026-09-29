begin transaction;

create table auth_user
(
    id            uuid         not null primary key                               default gen_random_uuid(),
    email         varchar(255) not null unique,
    password_hash varchar(255) not null,
    role          varchar(10)  not null check (role = 'Admin' or role = 'Client') default 'Client',
    created_at    timestamptz  not null                                           default current_timestamp,
    updated_at    timestamptz  not null                                           default now()
);

create table user_profile
(
    user_id    uuid         not null primary key references auth_user (id) on delete cascade,
    first_name varchar(50)  not null,
    last_name  varchar(50)  not null,
    bio        varchar(150) not null,
    avatar_url varchar(255) not null,
    updated_at timestamptz  not null default now()
);

create table refresh_token
(
    id                uuid         not null primary key default gen_random_uuid(),
    user_id           uuid         not null references auth_user (id) on delete cascade,
    token             varchar(100) not null,
    token_expires     timestamptz         not null,
    revoked_at        timestamptz,
    replaced_by_token varchar(100)
);

create or replace function update_updated_at_column()
    returns trigger as
$$
begin
    new.updated_at = now();
    return new;
end;
$$ language plpgsql;

-- 1. Creating the cleanup function
/*create or replace function purge_expired_tokens()
returns trigger as
$$
begin
    -- Deletes any token where the expiration time has passed
    delete from refresh_token where token_expires < now();
    return new;
end;
$$ language plpgsql;*/

create trigger auth_user_timestamp_update
    before update
    on auth_user
    for each row
execute function update_updated_at_column();

create trigger user_profile_timestamp_update
    before update
    on user_profile
    for each row
execute function update_updated_at_column();

-- 2. Binding the trigger to the table
/*create trigger trigger_purge_tokens
    after insert on refresh_token
    for each statement
execute function purge_expired_tokens();*/

commit;