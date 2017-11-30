using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OptimalPath : MonoBehaviour {

  Graph space;
  public Stack<Vector2> reversedAutoPath;
  public Vector2 targetA;

  void Start() {
    space = new Graph();
		reversedAutoPath = new Stack<Vector2>();
  }

  	// AutoPathing Script
	private class Graph {
    public HashSet<Vertex> vertices = new HashSet<Vertex>();
    public Dictionary<Vertex, HashSet<Vertex>> edges = new Dictionary<Vertex, HashSet<Vertex>>();

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
      else if (this.priority > other.priority) return 1;
      return -1;
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

  public Stack<Vector2> GenerateGraph(Vector2 targetA) {
    space = new Graph();
    float xDist = targetA.x - transform.position.x;
    float yDist = targetA.y - transform.position.y;
    float currentX = transform.position.x;
    float startingY = transform.position.y;
    if (xDist > 0) {
      while (targetA.x - currentX > .2f) {
        float currentY = startingY;
        if (yDist > 0) {
          while (3f - currentY > .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
						if (!GameObject.Find("Obstacle").GetComponent<Collider>().bounds.Contains(point.data)) {
							space.addVertex(point);
						}
            currentY = currentY + .2f;
          }
        } else {
          while (3f - currentY < .2f) {
            Vertex point = new Vertex(new Vector2(currentX, currentY));
						if (!GameObject.Find("Obstacle").GetComponent<Collider>().bounds.Contains(point.data)) {
							space.addVertex(point);
						}
            currentY = currentY - .2f;
          }
        }
        currentX = currentX + .2f;
      }
    } else {
      while (targetA.x - currentX < .2f) {
        float currentY = startingY;
        if (yDist > 0) {
          while (3f - currentY > .2f) {
						Vertex point = new Vertex(new Vector2(currentX, currentY));
						if (!GameObject.Find("Obstacle").GetComponent<Collider>().bounds.Contains(point.data)) {
							space.addVertex(point);
						}
            currentY = currentY + .2f;
          }
        } else {
          while (3f - currentY < .2f) {
						Vertex point = new Vertex(new Vector2(currentX, currentY));
						if (!GameObject.Find("Obstacle").GetComponent<Collider>().bounds.Contains(point.data)) {
							space.addVertex(point);
						}
            currentY = currentY - .2f;
          }
        }
        currentX = currentX - .2f;
      }
    }
		Vertex dest = new Vertex(targetA);
		if (!GameObject.Find("Obstacle").GetComponent<Collider>().bounds.Contains(dest.data)) {
			space.addVertex(dest);
		}
    space.populateEdges();
  	reversedAutoPath = AStar(space.getVertex(new Vector2(transform.position.x, transform.position.y)), space.getVertex(targetA));
		return reversedAutoPath;
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
    // Debug.Log("path: " + pathString);
    // Debug.Log("count: " + count);
		return reversedPath;
  }

	private float heuristic(Vertex a, Vertex b) {
		return Vector2.Distance(a.data, b.data);
  }

  // AStar Seach Algorithm
  private Stack<Vector2> AStar(Vertex start, Vertex dest) {

		if (start == null || dest == null) return new Stack<Vector2>();

		// Debug.Log("Start: " + start.data);
    // Debug.Log("Dest: " + dest.data);

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
    return new Stack<Vector2>();
  }
}
