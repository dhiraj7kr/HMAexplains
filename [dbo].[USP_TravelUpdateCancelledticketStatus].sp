
/****** Object:  StoredProcedure [dbo].[USP_TravelUpdateCancelledticketStatus]    Script Date: 16-02-2026 21:24:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_TravelUpdateCancelledticketStatus]
(
			   @status NVARCHAR(20)=NULL,
               @HMATravelID NVARCHAR(20)=NULL, 
               @FirstName NVARCHAR(20)=NULL,
			   @LastName NVARCHAR(20)=NULL,
			    @totalFare NVARCHAR(20)=NULL,
				@JourneyType nvarchar(max)=null,
				@OneWayStatus nvarchar(max)=null,
				@TwoWayStatus nvarchar(max)=null
				)
AS
BEGIN
    
	if(@JourneyType='ONE_WAY')
	begin
	update tblPassengers set Status=9,onewaystatus=9, ModifiedDate=GETDATE() 
	where firstname=@FirstName and LastName=@LastName and 
	TravelRequest_Id=@HMATravelID and onewaystatus=6
	--update tblTravelRequest set Status=15 ,ModifiedDate=GETDATE(),TotalCost=@totalFare where Id=@HMATravelID
	end
	else
	begin
	if(@OneWayStatus='9')
	begin
	update tblPassengers set onewaystatus=9, ModifiedDate=GETDATE() 
	where firstname=@FirstName and LastName=@LastName and 
	TravelRequest_Id=@HMATravelID and onewaystatus=6

	update tblPassengers set Status=9 where firstname=@FirstName and LastName=@LastName and 
	TravelRequest_Id=@HMATravelID and onewaystatus=9 and Twowaystatus=9
	end
	
	if(@TwoWayStatus='9')
	update tblPassengers set Twowaystatus=9, ModifiedDate=GETDATE() 
	where firstname=@FirstName and LastName=@LastName and 
	TravelRequest_Id=@HMATravelID and Twowaystatus=6

		update tblPassengers set Status=9 where firstname=@FirstName and LastName=@LastName and 
	TravelRequest_Id=@HMATravelID and onewaystatus=9 and Twowaystatus=9
	end
END
