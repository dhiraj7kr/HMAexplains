/****** Object:  StoredProcedure [dbo].[USP_UpdateStatustoDraft]    Script Date: 17-02-2026 15:58:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_UpdateStatustoDraft]
(
			   
               @HMATravelID NVARCHAR(20)=NULL, 
                @Id int =null

				)
AS
BEGIN
    
	update tblPassengers set Status=14,onewaystatus=14, ModifiedDate=GETDATE() where Status!=5 and TravelRequest_Id=@Id
	update tblPassengers set Status=14,Twowaystatus=14,onewaystatus=14, ModifiedDate=GETDATE() where Status!=5 and TravelRequest_Id=@Id and Twowaystatus is not null
	update tblTravelRequest set Status=14 ,ModifiedDate=GETDATE() where Id=@Id
END
