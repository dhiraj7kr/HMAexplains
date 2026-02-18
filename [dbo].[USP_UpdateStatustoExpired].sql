/****** Object:  StoredProcedure [dbo].[USP_UpdateStatustoExpired]    Script Date: 17-02-2026 12:12:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_UpdateStatustoExpired]
(
			   
               @HMATravelID NVARCHAR(20)=NULL, 
                @Id int =null

				)
AS
BEGIN
    
	update tblPassengers set Status=12,ModifiedDate=GETDATE() where Status!=5 and TravelRequest_Id=@Id

	update tblTravelRequest set Status=12 ,ModifiedDate=GETDATE() where Id=@Id
END
