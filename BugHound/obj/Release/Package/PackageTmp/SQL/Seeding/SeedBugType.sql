-- Seed BugHound Role Lookup Table
-- 
-- Written:
--    by: Peter Smith
--    On: Oct. 24, 2014

USE [BugHoundSQL]
GO


INSERT INTO [dbo].[BugType]
(
	Name
)
VALUES
(
	'Result unexpected'
),
(
	'Handled error, execution continues'
),
(
	'Unhanded error, execution halted'
)
