# 로그 불러오기
# 프로시저 삭제
Drop Procedure GetLogMessages;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetLogMessages;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetLogMessages(
    IN in_offset INT,
    IN in_maxCnt INT,
    IN in_asc BOOL,
    IN in_userid VARCHAR(255),
    OUT return_count INT,
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
        SET return_count = -1;
    END;

	-- 전체 개수 조회
    SET @count_sql := CONCAT('SELECT COUNT(*) INTO @total_count FROM lllegaldrone.logdb WHERE TraineeID = ?;');

    -- Prepare statement for count
    PREPARE count_stmt FROM @count_sql;

    SET @UserID = in_userid;

    -- Execute count statement
    EXECUTE count_stmt USING @UserID;

    -- 전체 개수를 return_count에 설정
    SET return_count = @total_count;

    -- 기본 쿼리
    SET @order := CASE WHEN in_asc THEN 'ASC' ELSE 'DESC' END;

    SET @sql := CONCAT('SELECT 
							InstructorID, 
							TraineeID, 
							LogMessage, 
							LogWriteDateTime
						FROM 
							lllegaldrone.logdb
						WHERE 
							TraineeID = ?  -- 원하는 TraineeID로 변경
                        ORDER BY LogWriteDateTime ', @order, '
                        LIMIT ? OFFSET ?;');

    -- Prepare statement
    PREPARE stmt FROM @sql;

    SET @UserID = in_userid;
    SET @maxCnt = in_maxCnt;
    SET @offset = in_offset;

    -- Execute statement
    EXECUTE stmt USING @UserID, @maxCnt, @offset;
    
    -- Cleanup
    DEALLOCATE PREPARE stmt;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetLogMessages(0, 15, true, "yoyo5678", @return_count, @return_state);
Select @return_count;
Select @return_state;

#예시
SELECT 
	COUNT(*) as cnt,
    ROW_NUMBER() OVER (PARTITION BY DATE(LogWriteDateTime) ORDER BY LogWriteDateTime) AS idx,
    InstructorID, 
    TraineeID, 
    LogMessage, 
    LogWriteDateTime
FROM 
    lllegaldrone.logdb
WHERE 
    TraineeID = 'yoyo5678'  -- 원하는 TraineeID로 변경
ORDER BY 
    LogWriteDateTime ASC
    LIMIT 5 OFFSET 0;  -- 오름차순 정렬
    
    
    
    
    SELECT COUNT(*) as cnt FROM lllegaldrone.logdb WHERE TraineeID = "yoyo5678";
    
