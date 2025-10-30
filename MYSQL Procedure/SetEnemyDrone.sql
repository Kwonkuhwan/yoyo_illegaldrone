# 적 드론 설정
# 프로시저 삭제
Drop Procedure SetEnemyDrone;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS SetEnemyDrone;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE SetEnemyDrone(
    IN in_scenarioID VARCHAR(255),
    IN in_droneType INT,
    IN in_flyingOrder INT,
    IN in_flyingType INT,
    IN in_startpointX FLOAT,
    IN in_startpointY FLOAT,
    IN in_startpointZ FLOAT,
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 기본 쿼리
    INSERT INTO `lllegaldrone`.`enemydronedb`
    (`ScenarioID`,
    `DroneType`,
    `FlyingOrder`,
    `FlyingType`,
    `StartpointX`,
    `StartpointY`,
    `StartpointZ`,
    `OffensivePower`,
    `Attackerloadtime`,
    `MoveSpeed`)
    SELECT
        in_scenarioID,
        in_droneType,
        in_flyingOrder,
        in_flyingType,
        in_startpointX,
        in_startpointY,
        in_startpointZ,
        d.OffensivePower,
        d.Attackerloadtime,
        d.MoveSpeed
    FROM
        `lllegaldrone`.`enemydronedata` d
    WHERE
        d.DroneType = in_droneType;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL SetEnemyDrone("2321412", 1, 0, 0, 125.1, 30.5, 405.2, @return_state);
Select @return_state;

SELECT * FROM lllegaldrone.enemydronedb;

INSERT INTO `lllegaldrone`.`enemydronedb`
(`ScenarioID`,
`DroneType`,
`FlyingOrder`,
`FlyingType`,
`StartpointX`,
`StartpointY`,
`StartpointZ`,
`OffensivePower`,
`Attackerloadtime`,
`MoveSpeed`)
SELECT
    "2321412",
    0,
    0,
    0,
    125.1,
    30.5,
    405.2,
    d.OffensivePower,
    d.Attackreloadtime,
    d.MoveSpeed
FROM
    `lllegaldrone`.`enemydronedata` d
WHERE
    d.DroneType = 0;