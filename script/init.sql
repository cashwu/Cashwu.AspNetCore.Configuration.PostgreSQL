
create schema kv;

create table kv.configurationvalue
(
	key varchar(64) not null
		constraint configurationvalues_pk
			primary key,
	value varchar not null,
	lastupdated timestamptz default CURRENT_TIMESTAMP not null
);

create unique index configurationvalues_key_uindex
	on kv.configurationvalue (key);

create index configurationvalues_lastupdated_index
	on kv.configurationvalue (lastupdated desc);

CREATE OR REPLACE FUNCTION kv.trigger_set_timestamp()
    RETURNS TRIGGER AS $$
BEGIN
    NEW.lastupdated = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

create trigger set_timestamp
	before update
	on kv.configurationvalue
	for each row
	execute procedure kv.trigger_set_timestamp();

