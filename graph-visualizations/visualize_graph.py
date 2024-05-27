import json
import networkx as nx
from pyvis.network import Network
import argparse

# Parse command-line arguments
parser = argparse.ArgumentParser(description='Visualize a graph from a JSON file.')
parser.add_argument('graphFile', type=str, help='The JSON file containing the graph data')
args = parser.parse_args()

# Load JSON data
graphFile = args.graphFile
with open(graphFile, 'r') as file:
    data = json.load(file)

# Initialize the graph
G = nx.DiGraph()

# Parse the JSON data and add edges to the graph
for charge_point, ways in data.items():
    charge_point_data = json.loads(charge_point)
    from_node = charge_point_data['Barcode']

    for way in ways:
        to_node = way['to']['Barcode']
        distance = way['distanceInMeters']
        duration = way['duration']

        # Add edge to the graph
        G.add_edge(from_node, to_node, distance=distance, duration=duration)

# Initialize PyVis network
net = Network(notebook=False, height='100vh', width='100%', bgcolor='#222222', font_color='white')

# Convert NetworkX graph to PyVis network
net.from_nx(G)

# Set options for better visualization with adjusted physics settings
net.set_options("""
var options = {
  "nodes": {
    "color": {
      "border": "rgba(255,255,255,1)",
      "background": "rgba(97,195,238,1)"
    },
    "font": {
      "color": "rgba(255,255,255,1)"
    }
  },
  "edges": {
    "color": {
      "color": "rgba(255,255,255,0.6)"
    },
    "smooth": {
      "type": "continuous"
    }
  },
  "interaction": {
    "dragNodes": true,
    "hideEdgesOnDrag": false,
    "hideNodesOnDrag": false
  },
  "physics": {
    "enabled": true,
    "stabilization": {
      "iterations": 1000
    }
  }
}
""")


# Generate the HTML file
html_file = graphFile.replace('.json', '') + '.html'
net.write_html(html_file, notebook=False)

print(f"Graph visualization saved as {html_file}")
