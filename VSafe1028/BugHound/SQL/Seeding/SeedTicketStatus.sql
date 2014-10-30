-- Seed BugHound Ticket Status Lookup Table
-- 
-- Written:
--    by: Peter Smith
--    On: Oct. 24, 2014

USE [BugHoundSQL]
GO


INSERT INTO [dbo].[TicketStatus]
(
	Name
)
VALUES
(
	'Open'
),
(
	 'Closed'
),
(
	 'On Hold'
)
