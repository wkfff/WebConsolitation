/*******************************************************************
  Переводит базу Sql Server 2005 из версии 3.0 в следующую версию 3.X
*******************************************************************/

/* Start - Добавление интерфейса "Сбор информации для официального сайта ГМУ" - Shelpanov - 16.12.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (263, 'RIA.E86N', 'Сбор информации для официального сайта ГМУ', 'Krista.FM.RIA.Extensions.E86N.E86NExtensionInstaller, Krista.FM.RIA.Extensions.E86N');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (115, '3.0.0.19', CONVERT(datetime, '2011.12.16', 102), GETDATE(), 'Сбор информации для официального сайта ГМУ', 0);

go

/* End - Добавление интерфейса "Сбор информации для официального сайта ГМУ" - Shelpanov - 16.12.2011 */
