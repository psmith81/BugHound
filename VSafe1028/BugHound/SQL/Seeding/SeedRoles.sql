-- Seed BugHound Role Lookup Table
-- 
-- Written:
--    by: Peter Smith
--    On: Oct. 24, 2014

USE [BugHoundSQL]
GO


INSERT INTO [dbo].[AspNetRoles]
(
	Id,
	Name
)
VALUES
(
	NEWID(),
	'Support Level 2'
)