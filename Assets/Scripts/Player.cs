using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public float jumpPower;
	public int jumps;

	Graph space;
	Stack<Vector2> reversedAutoPath;

	public enum State {
		idle,
		running,
		autoPathing,
		dashing,
		slashing
	};

	public enum SlashType {
		upSlash,
		downSlash,
		jab,
		upJab,
		downJab
	}

	public State state;
	public SlashType slashType;
	public bool grounded;
	public bool autoPathing;
	public bool completedAutoPathing; // triggeers dash/slash after completed autopathing

	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash
	public SlashIndicator slashIndicator;
	private Rigidbody2D rb;
    private Animator anim;

	// constants
	public float SLASHING_X_DIST;
	public float SLASHING_Y_DIST;
	public float SLASHING_THRESHOLD;
	public float AUTO_JUMP_FACTOR;
	public float TURNING_THRESHOLD;
	public float KP;
	public float GRAVITY_SCALE;
	public float DASH_SPEED = 40f;
	public float JAB_THRESHOLD;

	private float slashStartTime;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
		state = State.idle;
		completedAutoPathing = false;
		space = new Graph();
	}
	
	// Update is called once per frame
	void Update () {
    
		// turn the sprite around
		if (rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (1, 1, 1);
			if (state == State.idle)
				state = State.running;
		} else if (rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
			if (state == State.idle)
				state = State.running;
		} else if (state == State.running) state = State.idle;

        anim.SetBool("grounded", grounded);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		// for move left and right manually
		if (Input.GetKey(key:KeyCode.A)) {
			CancelAutomation();
			rb.velocity += Vector2.left * speed;
			state = State.running;
		}
		else if (state == State.running) state = State.idle;

		if(Input.GetKey(key:KeyCode.D)) {
			CancelAutomation();
			rb.velocity += Vector2.right * speed;
			state = State.running;
		}
		else if (state == State.running) state = State.idle;

        // for jumping
        if (Input.GetKeyDown(key: KeyCode.W) && jumps > 0)
        {
			CancelAutomation();
			Jump(jumpPower);
			
            state = State.idle;
        }

        // if we clicked, start autopathing towards that direction
        if (Input.GetMouseButtonDown(0)) { // if left click
			jumps = 0;
            state = State.autoPathing;
						completedAutoPathing = false;
            targetA = MouseWorldPosition2D();
						reversedAutoPath = GenerateGraph(targetA);
			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		}
		else if (Input.GetMouseButton(0)) {
			if (Vector2.Distance(targetA, MouseWorldPosition2D()) > SLASHING_THRESHOLD)
				slashIndicator.spriteRenderer.color = Color.blue;
			else slashIndicator.spriteRenderer.color = Color.red;
		}
		else {
			rb.gravityScale = GRAVITY_SCALE;
		}

		// if we release the mouse click, then we have finished drawing a slash
		if (Input.GetMouseButtonUp(0)) {
			if (state == State.autoPathing && Vector3.Distance(transform.position, targetA) <= .3f) {
				CancelAutomation();
				state = State.idle;
			}

			targetB = MouseWorldPosition2D();
			if (Vector2.Distance(targetA, targetB) > SLASHING_THRESHOLD) {
				state = State.dashing;
				// dashing is handle on a frame-by-frame basis
			}
			else if (Vector2.Distance(targetA, targetB) > .2 ) {
				state = State.slashing;
				CalcSlashType(); 	// sets slashType to the correct type of slash
				Slash();			// start the slash
			}
		}

		// actions based on the state
		switch (state) {
			case State.autoPathing:
				AutoPath("none");
				break;
			
			case State.dashing:
				Dash();
				break;

			case State.slashing:
				// check if the slash is over by seeing if the current playing animation is idle
				if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") && Time.time > slashStartTime + 0.1) {
					Debug.Log("Slash ended!");
					rb.WakeUp();
					state = State.idle;
				}
				else rb.Sleep();

				break;

			default:
				break;
		}		

		// limit the speed
		if (rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if (rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}

	// method to handle the autopathing
	/// <param name="type"> label that tells function if it has a successor action </param>

	private int AutoPath(string type) {
		if (reversedAutoPath.Count == 0) {
			rb.gravityScale = 0;
			rb.velocity = new Vector3(0, 0);
			if (type == "none") state = State.idle;
			// state = State.idle;
			return 1;
		}

		Vector2 nextLocation = reversedAutoPath.Pop();

		float xDist = nextLocation.x - transform.position.x;
		float yDist = nextLocation.y - transform.position.y;

		transform.position = Vector2.MoveTowards(transform.position, nextLocation, DASH_SPEED * Time.deltaTime);
		return 0;
	}

		// float xDist = targetA.x - transform.position.x;
		// float yDist = targetA.y - transform.position.y;

		// if we are at the position to start slashing, freeze!
	// 	if(Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {

	// 		rb.gravityScale = 0;
	// 		rb.velocity = new Vector3(0, 0);
	// 		if (type == "none") state = State.idle;
	// 		// state = State.idle;
	// 		return 1;
	// 	}

	// 	// otherwise, if we need to move in the x direction, do so
	// 	if (Mathf.Abs(xDist) >= SLASHING_X_DIST) {
	// 		rb.velocity = new Vector2(xDist * KP, yDist * KP);
	// 	}
	// 	return 0;
	// }

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI() {
		GUI.Label(new Rect(20, 20, 100, 100), "Velocity = " + rb.velocity.ToString());
	}

	// method to handle dashing
	private void Dash() {
		float distance = Vector3.Distance(transform.position, targetB);
		if (completedAutoPathing == true) {
			if (distance > .8f) {
				transform.position = Vector2.MoveTowards(transform.position, targetB, DASH_SPEED * Time.deltaTime);
			} else {
				rb.gravityScale = 0;
				rb.velocity = new Vector3(0, 0, 0);
				state = State.idle;
				completedAutoPathing = false;
			}
		} else {
			if (AutoPath("dash") == 1) {
				completedAutoPathing = true;
			}
		}
	}

	private Vector2 MouseWorldPosition2D(){
		Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	}

	public void CancelAutomation() {
		if(state == State.autoPathing || state == State.dashing || state == State.slashing) {
				rb.velocity = new Vector3(0, 0, 0);
				rb.gravityScale = GRAVITY_SCALE;
			}
	}
	

	private void Jump(float power) {
		rb.velocity = new Vector2(rb.velocity.x, 0); // prevents stacking velocity
		rb.velocity += Vector2.up * power;
		jumps--;
	}

	// method to perform the slash
	private void Slash(){
		Debug.Log("Slashing");
		rb.Sleep();
		slashStartTime = Time.time;

		switch (slashType) {
			case SlashType.upJab:
				break;

			case SlashType.jab:
				break;

			case SlashType.downJab:
				break;

			case SlashType.upSlash:
			
				rb.gravityScale = 0;
				anim.Play("PlayerUpSlash");
				break;

			case SlashType.downSlash:
				break;
		}
	}

	// method to get the slash type based on targetA and targetB
	private void CalcSlashType(){
		Debug.Log("finding slash type");
		// if this is a jab
		if(Vector2.Distance(transform.position, targetA) < JAB_THRESHOLD) {
			float angle = Mathf.Atan2(targetB.y - targetA.y, 
				targetB.x - targetA.x) * 180 / Mathf.PI;
			Debug.Log("It's a jab! Angle = " + angle);

			if(angle > 30) slashType = SlashType.upJab;
			else if(angle < -30) slashType = SlashType.downJab;
			else slashType = SlashType.jab;
		}
		// otherwise this must be a slash
		else {
			Debug.Log("It's a slash!");
			if(targetA.y >= targetB.y) slashType = SlashType.downSlash;
			else slashType = SlashType.upSlash;
		}
	}


	// AutoPathing Script

	private class Graph {
    HashSet<Vertex> vertices = new HashSet<Vertex>();
    Dictionary<Vertex, HashSet<Vertex>> edges = new Dictionary<Vertex, HashSet<Vertex>>();

    public Graph() {}

    public void addEdge(Vertex pointA, Vertex pointB) {
      if (!edges.ContainsKey(pointA)) {
        HashSet<Vertex> list = new HashSet<Vertex>();
        edges[pointA] = list;
      }
      if (!edges.ContainsKey(pointB)) {
        HashSet<Vertex> list = new HashSet<Vertex>();
        edges[pointB] = list;
      }
      HashSet<Vertex> listA = edges[pointA];
      HashSet<Vertex> listB = edges[pointB];
      listA.Add(pointB);
      listB.Add(pointA);
      edges[pointA] = listA;
      edges[pointB] = listB;
    }

    public void addVertex(Vertex point) {
      vertices.Add(point);
    }

    public HashSet<Vertex> getEdges(Vertex point) {
      HashSet<Vertex> output = null;
      this.edges.TryGetValue(point, out output);
      return output;
    }

    public Vertex getVertex(Vector2 data) {
      foreach(Vertex item in this.vertices) {
        if (item.data == data) {
          return item;
        }
      }
      return null;
    }

    public void populateEdges() {
      Debug.Log(this.vertices.Count);
      foreach(Vertex item in this.vertices) {
        foreach(Vertex compareItem in this.vertices) {
          if (item != compareItem) {
            float distance = Vector2.Distance(item.data, compareItem.data);
            if (distance <= .5f) {
              this.addEdge(item, compareItem);
            }
          }
        }
      }
    }
  }

  private class Vertex : IComparable<Vertex> {
    public Vector2 data;
    public float priority;

    public Vertex(Vector2 data) {
      this.data = data;
    }

    public int CompareTo(Vertex other) {
      if (other == null) {
        return -1;
      }
      if (this.priority == other.priority) return 0;
      else if (this.priority > other.priority) return -1;
      return 1;
    }
  }

  private class PriorityQueue <T> where T : IComparable <T> {
    private List <T> data;

    public PriorityQueue()
    {
      this.data = new List <T>();
    }

    public void Enqueue(T item) {
      data.Add(item);
      int ci = data.Count - 1;
      while (ci  > 0)
      {
        int pi = (ci - 1) / 2;
        if (data[ci].CompareTo(data[pi]) >= 0)
          break;
        T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
        ci = pi;
      }
    }

    public T Dequeue() {
      // Assumes pq isn't empty
      int li = data.Count - 1;
      T frontItem = data[0];
      data[0] = data[li];
      data.RemoveAt(li);

      --li;
      int pi = 0;
      while (true)
      {
        int ci = pi * 2 + 1;
        if (ci  > li) break;
        int rc = ci + 1;
        if (rc  <= li && data[rc].CompareTo(data[ci])  < 0)
          ci = rc;
        if (data[pi].CompareTo(data[ci])  <= 0) break;
        T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
        pi = ci;
      }
      return frontItem;
    }

    public int Count() {
      return data.Count;
    }
  }

  private Stack<Vector2> GenerateGraph(Vector2 targetA) {
    space = new Graph();
    float xDist = targetA.x - transform.position.x;
    float yDist = targetA.y - transform.position.y;
    float currentX = transform.position.x;
    float startingY = transform.position.y;
    if (xDist > 0) {
      while (targetA.x - currentX > .2f) {
        float currentY = startingY;
        if (yDist > 0) {
          while (targetA.y - currentY > .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
            space.addVertex(point);
            currentY = currentY + .2f;
          }
        } else {
          while (targetA.y - currentY < .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
            space.addVertex(point);
            currentY = currentY - .2f;
          }
        }
        currentX = currentX + .2f;
      }
    } else {
      while (targetA.x - currentX < .2f) {
        float currentY = startingY;
        if (yDist > 0) {
          while (targetA.y - currentY > .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
            space.addVertex(point);
            currentY = currentY + .2f;
          }
        } else {
          while (targetA.y - currentY < .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
            space.addVertex(point);
            currentY = currentY - .2f;
          }
        }
        currentX = currentX - .2f;
      }
    }
    space.addVertex(new Vertex(new Vector2(targetA.x, targetA.y)));
    space.populateEdges();
  	Stack<Vector2> list = a_star(space.getVertex(new Vector2(transform.position.x, transform.position.y)), space.getVertex(targetA));
		return list;
  }

  private Stack<Vector2> backchain(Dictionary<Vertex, Vertex> path, Vertex destination) {
    string pathString = "";
    pathString = pathString + destination.data + " -> ";
    Vertex current = path[destination];
    int count = 0;
		Stack<Vector2> reversedPath = new Stack<Vector2>();
		reversedPath.Push(destination.data);

    while (current != null) {
      count++;
      pathString = pathString + current.data + " -> ";
			reversedPath.Push(current.data);
      current = path[current];
    }
    Debug.Log("path: " + pathString);
    Debug.Log("count: " + count);
		return reversedPath;
  }

  // AStar Seach Algorithm
  private Stack<Vector2> a_star(Vertex start, Vertex dest) {

    Debug.Log("Start: " + start.data);
    Debug.Log("Dest: " + dest.data);

    // Queue<Vertex> frontier = new Queue<Vertex>();
    PriorityQueue<Vertex> frontier = new PriorityQueue<Vertex>();
    start.priority = 0;
    frontier.Enqueue(start);
    Dictionary<Vertex, Vertex> cameFrom = new Dictionary<Vertex, Vertex>();
    Dictionary<Vertex, float> costSoFar = new Dictionary<Vertex, float>();
    cameFrom[start] = null;
    costSoFar[start] = 0;

    while (frontier.Count() > 0) {
      Vertex current = frontier.Dequeue();
      if (current.data == dest.data) {
        Debug.Log("found the solution");
        return backchain(cameFrom, dest);
      }
      foreach (Vertex next in space.getEdges(current)) {
        float newCost = costSoFar[current] + Vector2.Distance(current.data, next.data);
        if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
          costSoFar[next] = newCost;
          next.priority = newCost + heuristic(dest, next);
          frontier.Enqueue(next);
          cameFrom[next] = current;
        }
      }
    }
    return null;
    
  }

  private float heuristic(Vertex a, Vertex b) {
   float dx = Mathf.Abs(a.data.x - b.data.x);
   float dy = Mathf.Abs(a.data.y - b.data.y);
  //  return Mathf.Sqrt(dx * dx + dy * dy);
  return dx + dy;
  }
}
