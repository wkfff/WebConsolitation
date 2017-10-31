echo off
chcp 1251
cd %1
set ORACLE_SID=DV
sqlplus /nolog @MakeDatabase.sql