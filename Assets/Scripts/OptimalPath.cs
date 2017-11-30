using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OptimalPath : MonoBehaviour {

  // Graph space;
  // public Vector2 targetA;

  // private class Graph {
  //   HashSet<Vertex> vertices = new HashSet<Vertex>();
  //   Dictionary<Vertex, HashSet<Vertex>> edges = new Dictionary<Vertex, HashSet<Vertex>>();

  //   public Graph() {}

  //   public void addEdge(Vertex pointA, Vertex pointB) {
  //     if (!edges.ContainsKey(pointA)) {
  //       HashSet<Vertex> list = new HashSet<Vertex>();
  //       edges[pointA] = list;
  //     }
  //     if (!edges.ContainsKey(pointB)) {
  //       HashSet<Vertex> list = new HashSet<Vertex>();
  //       edges[pointB] = list;
  //     }
  //     HashSet<Vertex> listA = edges[pointA];
  //     HashSet<Vertex> listB = edges[pointB];
  //     listA.Add(pointB);
  //     listB.Add(pointA);
  //     edges[pointA] = listA;
  //     edges[pointB] = listB;
  //   }

  //   public void addVertex(Vertex point) {
  //     vertices.Add(point);
  //   }

  //   public HashSet<Vertex> getEdges(Vertex point) {
  //     HashSet<Vertex> output = null;
  //     this.edges.TryGetValue(point, out output);
  //     return output;
  //   }

  //   public Vertex getVertex(Vector2 data) {
  //     foreach(Vertex item in this.vertices) {
  //       if (item.data == data) {
  //         return item;
  //       }
  //     }
  //     return null;
  //   }

  //   public void populateEdges() {
  //     Debug.Log(this.vertices.Count);
  //     foreach(Vertex item in this.vertices) {
  //       foreach(Vertex compareItem in this.vertices) {
  //         if (item != compareItem) {
  //           float distance = Vector2.Distance(item.data, compareItem.data);
  //           if (distance <= .5f) {
  //             this.addEdge(item, compareItem);
  //           }
  //         }
  //       }
  //     }
  //   }
  // }

  // private class Vertex : IComparable<Vertex> {
  //   public Vector2 data;
  //   public float priority;

  //   public Vertex(Vector2 data) {
  //     this.data = data;
  //   }

  //   public int CompareTo(Vertex other) {
  //     if (other == null) {
  //       return -1;
  //     }
  //     if (this.priority == other.priority) return 0;
  //     else if (this.priority > other.priority) return -1;
  //     return 1;
  //   }
  // }

  // private class PriorityQueue <T> where T : IComparable <T> {
  //   private List <T> data;

  //   public PriorityQueue()
  //   {
  //     this.data = new List <T>();
  //   }

  //   public void Enqueue(T item) {
  //     data.Add(item);
  //     int ci = data.Count - 1;
  //     while (ci  > 0)
  //     {
  //       int pi = (ci - 1) / 2;
  //       if (data[ci].CompareTo(data[pi]) >= 0)
  //         break;
  //       T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
  //       ci = pi;
  //     }
  //   }

  //   public T Dequeue() {
  //     // Assumes pq isn't empty
  //     int li = data.Count - 1;
  //     T frontItem = data[0];
  //     data[0] = data[li];
  //     data.RemoveAt(li);

  //     --li;
  //     int pi = 0;
  //     while (true)
  //     {
  //       int ci = pi * 2 + 1;
  //       if (ci  > li) break;
  //       int rc = ci + 1;
  //       if (rc  <= li && data[rc].CompareTo(data[ci])  < 0)
  //         ci = rc;
  //       if (data[pi].CompareTo(data[ci])  <= 0) break;
  //       T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
  //       pi = ci;
  //     }
  //     return frontItem;
  //   }

  //   public int Count() {
  //     return data.Count;
  //   }
  // }

  // void Start() {
  // }

  // void FixedUpdate() {
  //   if (Input.GetMouseButtonDown(0)) {
  //     targetA = MouseWorldPosition2D();
  //     GenerateGraph(targetA);
  //   }
  // }

  // private void GenerateGraph(Vector2 targetA) {
  //   space = new Graph();
  //   float xDist = targetA.x - transform.position.x;
  //   float yDist = targetA.y - transform.position.y;
  //   float currentX = transform.position.x;
  //   float startingY = transform.position.y;
  //   if (xDist > 0) {
  //     while (targetA.x - currentX > .2f) {
  //       float currentY = startingY;
  //       if (yDist > 0) {
  //         while (targetA.y - currentY > .2f) {
  //           Vertex point = new Vertex(new Vector2(currentX, currentY));
  //           space.addVertex(point);
  //           currentY = currentY + .2f;
  //         }
  //       } else {
  //         while (targetA.y - currentY < .2f) {
  //           Vertex point = new Vertex(new Vector2(currentX, currentY));
  //           space.addVertex(point);
  //           currentY = currentY - .2f;
  //         }
  //       }
  //       currentX = currentX + .2f;
  //     }
  //   } else {
  //     while (targetA.x - currentX < .2f) {
  //       float currentY = startingY;
  //       if (yDist > 0) {
  //         while (targetA.y - currentY > .2f) {
  //           Vertex point = new Vertex(new Vector2(currentX, currentY));
  //           space.addVertex(point);
  //           currentY = currentY + .2f;
  //         }
  //       } else {
  //         while (targetA.y - currentY < .2f) {
  //           Vertex point = new Vertex(new Vector2(currentX, currentY));
  //           space.addVertex(point);
  //           currentY = currentY - .2f;
  //         }
  //       }
  //       currentX = currentX - .2f;
  //     }
  //   }
  //   space.addVertex(new Vertex(new Vector2(targetA.x, targetA.y)));
  //   space.populateEdges();
  //   Dictionary<Vertex, Vertex> list = a_star(space.getVertex(new Vector2(transform.position.x, transform.position.y)), space.getVertex(targetA));
  // }

  // private void backchain(Dictionary<Vertex, Vertex> path, Vertex destination) {
  //   string pathString = "";
  //   pathString = pathString + destination.data + " -> ";
  //   Vertex current = path[destination];
  //   int count = 0;

  //   while (current != null) {
  //     count++;
  //     pathString = pathString + current.data + " -> ";
  //     current = path[current];
  //   }
  //   Debug.Log("path: " + pathString);
  //   Debug.Log("count: " + count);
  // }

  // // AStar Seach Algorithm
  // private Dictionary<Vertex, Vertex> a_star(Vertex start, Vertex dest) {

  //   Debug.Log("Start: " + start.data);
  //   Debug.Log("Dest: " + dest.data);

  //   // Queue<Vertex> frontier = new Queue<Vertex>();
  //   PriorityQueue<Vertex> frontier = new PriorityQueue<Vertex>();
  //   start.priority = 0;
  //   frontier.Enqueue(start);
  //   Dictionary<Vertex, Vertex> cameFrom = new Dictionary<Vertex, Vertex>();
  //   Dictionary<Vertex, float> costSoFar = new Dictionary<Vertex, float>();
  //   cameFrom[start] = null;
  //   costSoFar[start] = 0;

  //   while (frontier.Count() > 0) {
  //     Vertex current = frontier.Dequeue();
  //     if (current.data == dest.data) {
  //       Debug.Log("found the solution");
  //       backchain(cameFrom, dest);
  //       break;
  //     }
  //     foreach (Vertex next in space.getEdges(current)) {
  //       float newCost = costSoFar[current] + Vector2.Distance(current.data, next.data);
  //       if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
  //         costSoFar[next] = newCost;
  //         next.priority = newCost + heuristic(dest, next);
  //         frontier.Enqueue(next);
  //         cameFrom[next] = current;
  //       }
  //     }
  //   }
  //   return cameFrom;
    
  // }

  // private float heuristic(Vertex a, Vertex b) {
  //  float dx = Mathf.Abs(a.data.x - b.data.x);
  //  float dy = Mathf.Abs(a.data.y - b.data.y);
  // //  return Mathf.Sqrt(dx * dx + dy * dy);
  // return dx + dy;
  // }

  // private Vector2 MouseWorldPosition2D() {
	// 	Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	// 	return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	// }

}
