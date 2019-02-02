/* alter table [File] add [Description] VARCHAR (MAX) NULL*/
update  [file] set description = 'test' where id = 75
select * from [file]