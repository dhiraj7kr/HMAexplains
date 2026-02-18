/****** Object:  StoredProcedure [dbo].[USP_GetPendingApprovalTravelRequests]    Script Date: 16-02-2026 18:30:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_GetPendingApprovalTravelRequests]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
SELECT HMARequestId,OneWaydepartureDate_1,TicketStatus,HMATravelRequestID,Status,journeyType,ID FROM tblTravelRequest (nolock)where Status in (3)


END
