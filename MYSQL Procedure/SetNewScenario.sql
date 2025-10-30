# 새 시나리오 작성
# 프로시저 삭제
Drop Procedure SetNewScenario;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS SetNewScenario;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE SetNewScenario(
    IN in_scenarioID VARCHAR(255),
    IN in_playMode INT,
    IN in_playingNumber INT,
    IN in_missionMap INT,
    IN in_weather INT,
    IN in_timeZone INT,
    IN in_defenseAreaType INT,
    IN in_defenseArea INT,
    IN in_defenseObjectHP FLOAT,
    IN in_defenseObjectlocationPointX FLOAT,
    IN in_defenseObjectlocationPointY FLOAT,
    IN in_defenseObjectlocationPointZ FLOAT,
    IN in_firstPlayerLocationPointX FLOAT,
    IN in_firstPlayerLocationPointY FLOAT,
    IN in_firstPlayerLocationPointZ FLOAT,
    IN in_secondPlayerLocationPointX FLOAT,
    IN in_secondPlayerLocationPointY FLOAT,
    IN in_secondPlayerLocationPointZ FLOAT,
    IN in_thirdPlayerLocationPointX FLOAT,
    IN in_thirdPlayerLocationPointY FLOAT,
    IN in_thirdPlayerLocationPointZ FLOAT,
    IN in_fourthPlayerLocationPointX Float,
    IN in_fourthPlayerLocationPointY Float,
    IN in_fourthPlayerLocationPointZ Float,
    IN in_playTime INT,
    IN in_limitPlayTime INT,
    IN in_CreateUser VARCHAR(255),
    IN in_endScenarioPlayID VARCHAR(255),
    OUT return_state INT
)
BEGIN
    -- 예외 발생 시 처리
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
    END;

    INSERT INTO `lllegaldrone`.`scenariodb`
    (
        `ScenarioID`,
        `CreateScenarioDateTime`,
        `ExpirationScenarioDate`,
        `PlayMode`,
        `PlayingNumber`,
        `MissionMap`,
        `Weather`,
        `TimeZone`,
        `DefenseAreaType`,
        `DefenseArea`,
        `DefenseObjectHP`,
        `DefenseObjectlocationPointX`,
        `DefenseObjectlocationPointY`,
        `DefenseObjectlocationPointZ`,
        `FirstPlayerLocationPointX`,
        `FirstPlayerLocationPointY`,
        `FirstPlayerLocationPointZ`,
        `SecondPlayerLocationPointX`,
        `SecondPlayerLocationPointY`,
        `SecondPlayerLocationPointZ`,
        `ThirdPlayerLocationPointX`,
        `ThirdPlayerLocationPointY`,
        `ThirdPlayerLocationPointZ`,
        `FourthPlayerLocationPointX`,
        `FourthPlayerLocationPointY`,
        `FourthPlayerLocationPointZ`,
        `Playtime`,
        `limitPlaytime`,
        `CreateUser`,
        `EndScenarioPlayID`
    )
    VALUES
    (
        in_scenarioID,
        NOW(),
        DATE_ADD(NOW(), INTERVAL 30 DAY),
        in_playMode,
        in_playingNumber,
        in_missionMap,
        in_weather,
        in_timeZone,
        in_defenseAreaType,
        in_defenseArea,
        in_defenseObjectHP,
        in_defenseObjectlocationPointX,
        in_defenseObjectlocationPointY,
        in_defenseObjectlocationPointZ,
        in_firstPlayerLocationPointX,
        in_firstPlayerLocationPointY,
        in_firstPlayerLocationPointZ,
        in_secondPlayerLocationPointX,
        in_secondPlayerLocationPointY,
        in_secondPlayerLocationPointZ,
        in_thirdPlayerLocationPointX,
        in_thirdPlayerLocationPointY,
        in_thirdPlayerLocationPointZ,
        in_fourthPlayerLocationPointX,
        in_fourthPlayerLocationPointY,
        in_fourthPlayerLocationPointZ,
        in_playTime,
        in_limitPlayTime,
        in_CreateUser,
        in_endScenarioPlayID
    );

    SET return_state = 1;  -- 성공 상태
END //

DELIMITER ;

CALL SetNewScenario(
    '2421515',  -- ScenarioID
    1,                -- PlayMode
    4,                -- PlayingNumber
    2,                -- MissionMap
    3,                -- Weather
    1,                -- TimeZone
    1,                -- DefenseAreaType
    1,				 -- DefenseArea
    1000.0,          -- DefenseObjectHP
    10.0,            -- DefenseObjectlocationPointX
    20.0,            -- DefenseObjectlocationPointY
    5.0,             -- DefenseObjectlocationPointZ
    1.0,             -- FirstPlayerLocationPointX
    1.0,             -- FirstPlayerLocationPointY
    0.0,             -- FirstPlayerLocationPointZ
    2.0,             -- SecondPlayerLocationPointX
    2.0,             -- SecondPlayerLocationPointY
    0.0,             -- SecondPlayerLocationPointZ
    3.0,             -- ThirdPlayerLocationPointX
    3.0,             -- ThirdPlayerLocationPointY
    0.0,             -- ThirdPlayerLocationPointZ
    4.0,             -- FourthPlayerLocationPointX
    4.0,             -- FourthPlayerLocationPointY
    0.0,             -- FourthPlayerLocationPointZ
    60,              -- PlayTime (분)
    120,             -- LimitPlayTime (분)
    '권구환',         -- CreateUser
    '아무개', -- EndScenarioPlayID
    @return_state    -- OUT 매개변수
);

-- 결과 확인
SELECT @return_state AS return_state;

# 예시
INSERT INTO `lllegaldrone`.`scenariodb`
(
	`ScenarioID`,
	`CreateScenarioDateTime`,
	`ExpirationScenarioDate`,
	`PlayMode`,
	`PlayingNumber`,
	`MissionMap`,
	`Weather`,
	`TimeZone`,
	`DefenseAreaType`,
	`DefenseObjectHP`,
	`DefenseObjectlocationPointX`,
	`DefenseObjectlocationPointY`,
	`DefenseObjectlocationPointZ`,
	`FirstPlayerLocationPointX`,
	`FirstPlayerLocationPointY`,
	`FirstPlayerLocationPointZ`,
	`SecondPlayerLocationPointX`,
	`SecondPlayerLocationPointY`,
	`SecondPlayerLocationPointZ`,
	`ThirdPlayerLocationPointX`,
	`ThirdPlayerLocationPointY`,
	`ThirdPlayerLocationPointZ`,
	`FourthPlayerLocationPointX`,
	`FourthPlayerLocationPointY`,
	`FourthPlayerLocationPointZ`,
	`Playtime`,
	`limitPlaytime`,
	`CreateUser`,
	`EndScenarioPlayID`
)
VALUES
(
	'12455125',  -- ScenarioID
	NOW(),
	DATE_ADD(NOW(), INTERVAL 30 DAY),    
	1,                -- PlayMode
	4,                -- PlayingNumber
	2,                -- MissionMap
	3,                -- Weather
	1,                -- TimeZone
	1,                -- DefenseAreaType
	1000.0,          -- DefenseObjectHP
	10.0,            -- DefenseObjectlocationPointX
	20.0,            -- DefenseObjectlocationPointY
	5.0,             -- DefenseObjectlocationPointZ
	1.0,             -- FirstPlayerLocationPointX
	1.0,             -- FirstPlayerLocationPointY
	0.0,             -- FirstPlayerLocationPointZ
	2.0,             -- SecondPlayerLocationPointX
	2.0,             -- SecondPlayerLocationPointY
	0.0,             -- SecondPlayerLocationPointZ
	3.0,             -- ThirdPlayerLocationPointX
	3.0,             -- ThirdPlayerLocationPointY
	0.0,             -- ThirdPlayerLocationPointZ
	4.0,             -- FourthPlayerLocationPointX
	4.0,             -- FourthPlayerLocationPointY
	0.0,             -- FourthPlayerLocationPointZ
	60,              -- PlayTime (분)
	120,             -- LimitPlayTime (분)
	'권구환',         -- CreateUser
	'아무개' -- EndScenarioPlayID
);