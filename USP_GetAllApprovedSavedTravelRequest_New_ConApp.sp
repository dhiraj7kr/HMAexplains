/****** Object:  StoredProcedure [dbo].[USP_GetAllApprovedSavedTravelRequest_New_ConApp]    Script Date: 17-02-2026 11:15:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_GetAllApprovedSavedTravelRequest_New_ConApp]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

		-- Insert statements for procedure here
	SELECT HMARequestId,TicketStatus,HMATravelRequestID,Status,journeyType,Id,Status as 'onewaystatus', Status as 'Twowaystatus',
	OneWaydepartureDate_1,ModifiedDate,Company FROM tblTravelRequest (nolock)where Status in (14,4,1) 
	--and OneWaydepartureDate_1>GETDATE()-30
	and CreatedDate>GETDATE()-180 --and ID >= 211701
	order by HMARequestId desc

END
