# 적 드론 검색
# 프로시저 삭제
Drop Procedure GetEnemyDrones;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetEnemyDrones;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetEnemyDrones(
    IN in_scenarioID VARCHAR(255),
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    SELECT * FROM lllegaldrone.enemydronedb
    WHERE ScenarioID = in_scenarioID;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetEnemyDrones("2321412", @return_state);

Select @return_state;

SELECT * FROM lllegaldrone.enemydronedb;

    SELECT * FROM lllegaldrone.enemydronedb
    WHERE ScenarioID = '2321412';