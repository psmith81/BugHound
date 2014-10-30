-- Seed BugHound Ticket Priorities Lookup Table
-- 
-- Written:
--    by: Peter Smith
--    On: Oct. 24, 2014

USE [BugHoundSQL]
GO


INSERT INTO [dbo].[Priorities]
(
	Name
)
VALUES
(
	'Low'
),
(
	'Medium'
),
(
	'High'
),
(
	'Critical'
)
