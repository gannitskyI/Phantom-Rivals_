using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CarAIHandler : MonoBehaviour
{ 

    public enum AIMode { FollowPlayer, FollowWaypoints, FollowMouse };

    [Header("Race settings")]
    public Collider2D finishLineTrigger; // Добавлено: триггер финишной черты
    public int totalLaps;// Добавлено: общее количество кругов
    [SerializeField]
    private int lapsCompleted = 0; // Добавлено: количество завершенных кругов
    public bool raceFinished = false; // Добавлено: флаг завершения гонки
    [SerializeField]
    private BotSpawner botSpawner;
    public bool finishLineTriggered = false;
    private float triggerDelay = 10f;

    [Header("AI settings")]
    public AIMode aiMode;
    public float maxSpeed = 16;
    public bool isAvoidingCars = true;
    private const float TeleportationThreshold = 0.1f;
    private const float TeleportationTimeout = 5f;
    public SpriteRenderer carSpriteRenderer;
    private float timeSinceLastMovement = 0.0f;
    [Range(0.0f, 1.0f)] public float skillLevel = 1.0f;
    [Range(0.1f, 2.0f)] public float corneringSpeedReduction = 0.5f;

    private Vector3 targetPosition = Vector3.zero;
    private Transform targetTransform = null;
    private float originalMaximumSpeed = 0;
    private bool isRunningStuckCheck = false;
    private bool isFirstTemporaryWaypoint = false;
    private int stuckCheckCounter = 0;
    private List<Vector2> temporaryWaypoints = new List<Vector2>();
    private float angleToTarget = 0;
    private Vector2 avoidanceVectorLerped = Vector3.zero;
    private WaypointNode currentWaypoint = null;
    private WaypointNode previousWaypoint = null;
    private WaypointNode[] allWayPoints;
    private CapsuleCollider2D capsuleCollider2D;
    private TopDownCarController topDownCarController;
    private AStarLite aStarLite;
    private TaskChecker taskChecker;

    private float minDistanceToReachWaypoint = 1.5f;
    private const float MinDistanceToReachWaypointFirst = 3.0f;
    private const float MaxAngleToReverse = 70;
    private const int MaxStuckCheckCount = 3;
    private const float AvoidanceRadius = 1.2f;
    private const float AvoidanceDistance = 12;
    private const float MaxSteeringAngle = 45.0f;
    private bool hasStartedMoving = false;
    private float timeSinceStart = 0;
    private CanvasFunction canvasFunction;


    private void Awake()
    {
        canvasFunction = FindObjectOfType<CanvasFunction>();
        topDownCarController = GetComponent<TopDownCarController>();
        allWayPoints = FindObjectsOfType<WaypointNode>();
        aStarLite = GetComponent<AStarLite>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        originalMaximumSpeed = maxSpeed;
    }

    private void Start()
    {
        taskChecker = FindObjectOfType<TaskChecker>();
        // Поиск объекта BotSpawner по имени
        GameObject botSpawnerObject = GameObject.Find("Bot");

        // Получение компонента BotSpawner из найденного объекта
        if (botSpawnerObject != null)
        {
            botSpawner = botSpawnerObject.GetComponent<BotSpawner>();
        }
        else
        {
            Debug.LogError("Ошибка: объект BotSpawner не найден.");
        }
        hasStartedMoving = false;
        SetMaxSpeedBasedOnSkillLevel(maxSpeed);
        Invoke("StartMoving", 3.0f);
        Invoke("FindStartLineBot", 5.0f);
        FindStartLineBot();
        
    }
  
    private void AvoidObstacles()
    {
        // Создаем список для хранения коллизий
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        // Проверяем коллизии впереди и сбоку от машины
        for (int i = -2; i <= 2; i++)
        {
            Vector2 direction = Quaternion.AngleAxis(i * 20, transform.up) * transform.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, AvoidanceRadius, LayerMask.GetMask("Cars"));

            if (hit.collider != null)
            {
                hits.Add(hit);
            }
        }

        // Если есть коллизии, вычисляем вектор избегания
        if (hits.Count > 0)
        {
            Vector2 avoidanceVector = Vector2.zero;

            foreach (RaycastHit2D hit in hits)
            {
                // Вычисляем вектор от машины до препятствия
                Vector2 difference = (Vector2)transform.position - hit.point;
                avoidanceVector += difference.normalized;
            }

            // Вычисляем новый вектор движения, используя вектор избегания
            Vector2 newDirection = (avoidanceVector / hits.Count).normalized;
            float angle = Vector2.SignedAngle(transform.up, newDirection);

            // Применяем поворот к машине
            topDownCarController.SetInputVector(new Vector2(angle / MaxSteeringAngle, 1.0f));
        }
    }

    private void StartMoving()
    {
        if (!canvasFunction.isCountdownFinished)
            return;

        hasStartedMoving = true;
    }

    private void FindStartLineBot()
    {
        finishLineTrigger = GameObject.Find("StartLineBot").GetComponent<Collider2D>();
    }


    private void FixedUpdate()
    {
        StartMoving();
        AvoidObstacles();
        if (!hasStartedMoving || raceFinished) return;
        CheckForTeleportation();

        switch (aiMode)
        {
            case AIMode.FollowPlayer: FollowPlayer(); break;
            case AIMode.FollowWaypoints: FollowWaypoints(); break;
            case AIMode.FollowMouse: FollowMousePosition(); break;
        }

        float inputX = TurnTowardTarget();
        float inputY = ApplyThrottleOrBrake(inputX);

        if (topDownCarController.GetVelocityMagnitude() < 0.5f && Mathf.Abs(inputY) > 0.01f && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCO());

        if (stuckCheckCounter >= MaxStuckCheckCount && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCO());

        topDownCarController.SetInputVector(new Vector2(inputX, inputY));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == finishLineTrigger)
        {
            HandleFinishLineEnter();
        }
    }

    private void HandleFinishLineEnter()
    {
        if (!finishLineTriggered)
        {
            finishLineTriggered = true;
            lapsCompleted++;

            if (lapsCompleted >= totalLaps)
            {
                hasStartedMoving = false;
                raceFinished = true;
                topDownCarController.SetInputVector(Vector2.zero);
                if (taskChecker != null)
                {
                    taskChecker.OneTask();
                }
            }

            // Запускаем корутину, которая сбросит флаг finishLineTriggered через заданное время
            StartCoroutine(ResetTrigger());
        }
    }

    private IEnumerator ResetTrigger()
    {
        // Ждем заданное количество секунд
        yield return new WaitForSeconds(triggerDelay);

        // Сбрасываем флаг
        finishLineTriggered = false;
        FindStartLineBot();
    }

    private void CheckForTeleportation()
    {
        if (topDownCarController.GetVelocityMagnitude() < TeleportationThreshold)
        {
            timeSinceLastMovement += Time.fixedDeltaTime;
            if (timeSinceLastMovement >= TeleportationTimeout)
            {
                TeleportToNearestCell();
                timeSinceLastMovement = 0.0f;
            }
        }
        else
        {
            timeSinceLastMovement = 0.0f;
        }
    }

    private void TeleportToNearestCell()
    {
        WaypointNode nearestWaypoint = FindClosestWayPoint();
        if (nearestWaypoint != null)
        {
            Sequence teleportSequence = DOTween.Sequence();
            teleportSequence.Append(carSpriteRenderer.DOFade(0, 0.5f))
                .AppendCallback(() => { transform.position = nearestWaypoint.transform.position; })
                .AppendInterval(3.0f)
                .Append(carSpriteRenderer.DOFade(0, 1.5f))
                .Join(carSpriteRenderer.DOFade(1, 1.5f))
                .Play();
        }
    }

    private void FollowPlayer()
    {
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (targetTransform != null)
            targetPosition = targetTransform.position;
    }

    private void FollowWaypoints()
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWayPoint();
            previousWaypoint = currentWaypoint;
        }

        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;

            if (distanceToWayPoint <= currentWaypoint.minDistanceToReachWaypoint * Random.Range(0.9f, 1.1f)) // добавляем небольшую вариацию в расстояние
            {
                if (currentWaypoint.maxSpeed > 0)
                    SetMaxSpeedBasedOnSkillLevel(currentWaypoint.maxSpeed);
                else
                    SetMaxSpeedBasedOnSkillLevel(1000);

                previousWaypoint = currentWaypoint;
                if (currentWaypoint.nextWaypointNode.Length > 0) // проверяем, что есть следующие точки маршрута
                {
                    // выбираем следующую точку маршрута на основе угла поворота, расстояния, кривизны трассы и скорости
                    float minAngle = float.MaxValue;
                    float minDistance = float.MaxValue;
                    float minCurvature = float.MaxValue;
                    float minSpeed = float.MaxValue;
                    WaypointNode bestNode = null;
                    foreach (var node in currentWaypoint.nextWaypointNode)
                    {
                        float angle = Vector2.Angle(transform.up, node.transform.position - transform.position); // угол между направлением автомобиля и направлением к точке маршрута
                        angle += Random.Range(-10f, 10f); // добавляем некоторый шум в угол
                        float distance = Vector2.Distance(transform.position, node.transform.position);
                        float curvature = Mathf.Abs(Vector2.Angle(transform.up, (node.transform.position - previousWaypoint.transform.position).normalized));
                        float speed = node.maxSpeed;

                        if (angle < minAngle || (angle == minAngle && distance < minDistance) || (angle == minAngle && distance == minDistance && curvature < minCurvature) || (angle == minAngle && distance == minDistance && curvature == minCurvature && speed < minSpeed))
                        {
                            minAngle = angle;
                            minDistance = distance;
                            minCurvature = curvature;
                            minSpeed = speed;
                            bestNode = node;
                        }
                    }
                    currentWaypoint = bestNode;
                }
            }
        }
    }

    private void FollowTemporaryWaypoints()
    {
        if (temporaryWaypoints.Count > 0) // проверяем, что список временных точек маршрута не пустой
        {
            targetPosition = temporaryWaypoints[0];
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;
            SetMaxSpeedBasedOnSkillLevel(originalMaximumSpeed * 0.5f * Random.Range(0.9f, 1.1f)); // устанавливаем более высокую максимальную скорость с небольшой вариацией

            if (!isFirstTemporaryWaypoint)
                minDistanceToReachWaypoint = currentWaypoint.minDistanceToReachWaypoint * 0.5f * Random.Range(0.9f, 1.1f); // устанавливаем более низкое минимальное расстояние до точки маршрута с небольшой вариацией

            if (distanceToWayPoint <= minDistanceToReachWaypoint)
            {
                temporaryWaypoints.RemoveAt(0);
                isFirstTemporaryWaypoint = false;
            }
        }
    }


    private void FollowMousePosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition = worldPosition;
    }

    private WaypointNode FindClosestWayPoint()
    {
        return allWayPoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    private float TurnTowardTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        // Check for obstacles and adjust target position
        if (IsObstacleAhead(out Vector3 obstaclePosition))
        {
            Vector3 avoidanceVector = (obstaclePosition - transform.position).normalized;
            vectorToTarget = Quaternion.AngleAxis(MaxSteeringAngle, Vector3.forward) * vectorToTarget; // Apply steering to avoid obstacle
        }

        angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        // Limit maximum steering angle
        float steerAmount = angleToTarget / MaxSteeringAngle;
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    private bool IsObstacleAhead(out Vector3 obstaclePosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, AvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider != null)
        {
            obstaclePosition = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.red);
            return true;
        }

        obstaclePosition = Vector3.zero;
        return false;
    }


    private float ApplyThrottleOrBrake(float inputX)
    {
        if (topDownCarController.GetVelocityMagnitude() > maxSpeed)
            return 0;

        float reduceSpeedDueToCornering = Mathf.Abs(inputX) / 1.0f;
        float throttle = 1.05f - reduceSpeedDueToCornering * skillLevel;

        if (temporaryWaypoints.Count != 0)
        {
            if (angleToTarget > MaxAngleToReverse || angleToTarget < -MaxAngleToReverse || stuckCheckCounter > MaxStuckCheckCount)
                throttle = throttle * -1;
        }

        // Увеличьте влияние угла поворота на снижение скорости
        float angleFactor = Mathf.Abs(angleToTarget) / MaxSteeringAngle;
        float angleInfluence = 1 - angleFactor * corneringSpeedReduction;
        throttle *= angleInfluence;

        return throttle;
    }

    private void SetMaxSpeedBasedOnSkillLevel(float newSpeed)
    {
        maxSpeed = Mathf.Clamp(newSpeed, 0, originalMaximumSpeed);
        float skillBasedMaximumSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed = maxSpeed * skillBasedMaximumSpeed;
    }

    private Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();
        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);
        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    private bool IsCarsInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector)
    {
        capsuleCollider2D.enabled = false;
        RaycastHit2D raycastHit2d = Physics2D.CircleCast(transform.position + transform.up * 0.5f, AvoidanceRadius, transform.up, AvoidanceDistance, 1 << LayerMask.NameToLayer("Car"));
        capsuleCollider2D.enabled = true;

        if (raycastHit2d.collider != null)
        {
            Debug.DrawRay(transform.position, transform.up * AvoidanceDistance, Color.red);
            position = raycastHit2d.collider.transform.position;
            otherCarRightVector = raycastHit2d.collider.transform.right;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.up * AvoidanceDistance, Color.black);
        }

        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;
        return false;
    }

    private void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarsInFrontOfAICar(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);
            float distanceToTarget = (targetPosition - transform.position).magnitude;
            float driveToTargetInfluence = 6.0f / distanceToTarget;
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.70f, 1.0f);
            float avoidanceInfluence = 1.0f - driveToTargetInfluence;
            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);
            newVectorToTarget = (vectorToTarget * driveToTargetInfluence + avoidanceVector * avoidanceInfluence);
            newVectorToTarget.Normalize();
            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);
            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);
            return;
        }

        newVectorToTarget = vectorToTarget;
    }

    private IEnumerator StuckCheckCO()
    {
        Vector3 initialStuckPosition = transform.position;
        isRunningStuckCheck = true;
        yield return new WaitForSeconds(1.0f); // Увеличено время ожидания перед проверкой на застревание

        if ((transform.position - initialStuckPosition).sqrMagnitude < 3)
        {
            temporaryWaypoints = aStarLite.FindPath(currentWaypoint.transform.position);
            if (temporaryWaypoints == null)
                temporaryWaypoints = new List<Vector2>();
            stuckCheckCounter++;
            isFirstTemporaryWaypoint = true;
        }
        else
        {
            stuckCheckCounter = 0;
        }

        isRunningStuckCheck = false;
    }
}