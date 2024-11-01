// AS YOU CAN SEE, NO REAL DATA SO FAR. TODO
let nodes = window.nodes;
let links = window.links;

const color_wheel = ["#ED3833", "#37BDB4", "#FF8100", "#9FB40A", "#FB4187"];

let graphData = {
  nodes: nodes,
  links: links,
};

let link_size = 100 + (100 / nodes.length);

let subnodes = [];
let sublinks = [];
let subgraphData = { nodes: subnodes, links: sublinks };

// Create a tooltip div
const tooltip = d3.select(".tooltip");
const node_info_string = d3.select(".node_info_string");
const node_info = d3.select(".node_info");

// Select the SVG element
let svg = d3.select(".svg_graph").attr("preserveAspectRatio", "xMidYMid meet");
let sub_svg = d3.select(".svg_subgraph");
const svg_container = document.querySelector(".svg_container");
const subsvg_container = document.querySelector(".subsvg_container");

// Define width and height
let width = svg_container.getAttribute("width");
let height = document.getElementById("svg_graph").getAttribute("height");
let subwidth = subsvg_container.clientWidth; // Get the container width
let subheight = subsvg_container.clientHeight;

let content = svg.append("g");
let zoom = d3
  .zoom()
  .scaleExtent([0.5, 10])
  .on("zoom", (event) => {
    content.attr("transform", event.transform);
  })
  .wheelDelta((event) => {
    return -event.deltaY * 0.001;
  });
svg.call(zoom);
svg.on("zoom", (event) => {
  event.preventDefault();
});

const simulation = d3
  .forceSimulation(graphData.nodes)
  .force(
    "link",
    d3
      .forceLink(graphData.links)
      .id((d) => d.id)
      .distance(link_size)
  )
  .force("charge", d3.forceManyBody().strength(-20))
  .force("center", d3.forceCenter(width / 2, height / 2))
  .force("edge", repelFromEdges()); // Custom force to push nodes from edges

getNodeColors(nodes);
//getLinkColors(links);
// Create lines for the links
let link = content
  .append("g")
  .selectAll("line")
  .data(graphData.links)
  .enter()
  .append("line")
  .attr("stroke", (d) => d.color);

// Create circles for the nodes, using the size and color fields, with tooltips
let node = content
  .append("g")
  .selectAll("circle")
  .data(graphData.nodes)
  .enter()
  .append("circle")
  .attr("r", (d) => scaleCircle(d.size))
  .attr("fill", (d) => d.color)
  .call(
    d3.drag().on("start", dragStarted).on("drag", dragged).on("end", dragEnded)
  );

// Add labels for the nodes
const label = content
  .append("g")
  .selectAll("text")
  .data(graphData.nodes)
  .enter()
  .append("text")
  .text((d) => d.text.length > 10 ? d.text.slice(0, 20) + " ..." : d.text)
  .style("font-size", "16px")
  .style("fill", "#F1EC91")
  .attr("dy", (d) => scaleCircle(d.size) + 15 + "px") // Vertically center the text
  .attr("text-anchor", "middle"); // Horizontally center the text

node
  .on("mouseover", function (event, d) {
    d3.select(this)
      .transition()
      .duration(200)
      .attr("fill", "white")
      .attr("r", (d) => scaleCircle(d.size) + 5);
    tooltip
      .style("display", "block") // TO ENABLE TOOLTIP CHANGE TO "block"
      .style("left", event.pageX + 10 + "px")
      .style("top", event.pageY - 10 + "px")
      .html(`Filename: ${d.filename}`);
  })
  .on("mouseout", function (event, d) {
    d3.select(this)
      .transition()
      .duration(200)
      .attr("fill", (d) => d.color)
      .attr("r", (d) => scaleCircle(d.size));

    tooltip.style("display", "none");
  })
  .on("click", (event, d) => {
    node_info.html(getHtmlString(d));
    //sendHttpRequest('GET', d.link).then((getData) => {d.file_text = getData;});
    selectNode(d);
  });
// Update positions on each tick of the simulation
simulation.on("tick", () => {
  link
    .attr("x1", (d) => d.source.x)
    .attr("y1", (d) => d.source.y)
    .attr("x2", (d) => d.target.x)
    .attr("y2", (d) => d.target.y);

  node.attr("cx", (d) => d.x).attr("cy", (d) => d.y);

  label.attr("x", (d) => d.x).attr("y", (d) => d.y);
});

// Drag event handlers
function dragStarted(event, d) {
  if (!event.active) simulation.alphaTarget(0.3).restart();
  d.fx = d.x;
  d.fy = d.y;
}

function dragged(event, d) {
  d.fx = event.x;
  d.fy = event.y;
}

function dragEnded(event, d) {
  if (!event.active) simulation.alphaTarget(0);
  d.fx = null;
  d.fy = null;
}
function resizeSvg() {
  width = svg_container.clientWidth; // Get the container width
  height = svg_container.clientHeight; // Get the container height
  svg.attr("width", width);
  svg.attr("height", height);
  simulation.force("center", d3.forceCenter(width / 2, height / 2));
  // Restart the simulation with the new center
  simulation.alpha(1).restart();

  subwidth = subsvg_container.clientWidth; // Get the container width
  subheight = subsvg_container.clientHeight; // Get the container height
  sub_svg.attr("width", subwidth);
  sub_svg.attr("height", subheight);
  console.log(subheight);
  subsimulation.force("center", d3.forceCenter(subwidth / 2, subheight / 2));
  //Restart the simulation with the new center
  subsimulation.alpha(1).restart();
}

window.addEventListener("load", resizeSvg);
window.addEventListener("resize", resizeSvg);

// Custom force to push nodes away from screen edges
function repelFromEdges(padding = 50, strength = 0.01) {
  return function () {
    nodes.forEach((node) => {
      // Force to repel nodes from left edge
      if (node.x < -3*width + padding) {
        node.vx += (-3*width + padding - node.x) * strength;
      }
      // Force to repel nodes from right edge
      if (node.x > 3*width - padding) {
        node.vx -= (node.x - (3 * width - padding)) * strength;
      }
      // Force to repel nodes from top edge
      if (node.y < -3 * height + padding) {
        node.vy += (-3 * height + padding - node.y) * strength;
      }
      // Force to repel nodes from bottom edge
      if (node.y > 3 * height - padding) {
        node.vy -= (node.y - (3 * height - padding)) * strength;
      }
    });
  };
}

function SUBrepelFromEdges(padding = 50, strength = 0.1) {
  return function () {
    subnodes.forEach((node) => {
      // Force to repel nodes from left edge
      if (node.x < padding) {
        node.vx += (padding - node.x) * strength;
      }
      // Force to repel nodes from right edge
      if (node.x > subwidth - padding) {
        node.vx -= (node.x - (subwidth - padding)) * strength;
      }
      // Force to repel nodes from top edge
      if (node.y < padding) {
        node.vy += (padding - node.y) * strength;
      }
      // Force to repel nodes from bottom edge
      if (node.y > subheight - padding) {
        node.vy -= (node.y - (subheight - padding)) * strength;
      }
    });
  };
}

function selectNode(node) {
  sub_svg.selectAll("circle").remove();
  sub_svg.selectAll("line").remove();
  sub_svg.selectAll("text").remove();
  subnodes.length = 0;
  sublinks.length = 0;
  subnodes.push(JSON.parse(JSON.stringify(node)));
  links.forEach((link) => {
    console.log(link);
    if (link.source.id == node.id) {
      subnodes.push(JSON.parse(JSON.stringify(link.target)));
      sublinks.push(JSON.parse(JSON.stringify(link)));
      sublinks[sublinks.length - 1].source = subnodes[0];
      sublinks[sublinks.length - 1].target = subnodes[subnodes.length - 1];
    }
    if (link.target.id == node.id) {
      subnodes.push(JSON.parse(JSON.stringify(link.source)));
      sublinks.push(JSON.parse(JSON.stringify(link)));
      sublinks[sublinks.length - 1].source = subnodes[0];
      sublinks[sublinks.length - 1].target = subnodes[subnodes.length - 1];
    }
  });
  subgraphData = { nodes: subnodes, links: sublinks };
  refreshSub();
}

function refreshSub() {
  subsimulation = d3
    .forceSimulation(subgraphData.nodes)
    .force(
      "link",
      d3
        .forceLink(subgraphData.links)
        .id((d) => d.id)
        .distance(100)
    )
    .force("charge", d3.forceManyBody().strength(-100))
    .force("center", d3.forceCenter(subwidth / 2, subheight / 2))
    .force("edge", SUBrepelFromEdges()); // Custom force to push nodes from edges
  // Create lines for the links
  sublink = sub_svg
    .selectAll("line")
    .data(subgraphData.links)
    .enter()
    .append("line")
    .attr("stroke", (d) => d.color);

  // Create circles for the nodes, using the size and color fields, with tooltips
  subnode = sub_svg
    .selectAll("circle")
    .data(subgraphData.nodes)
    .enter()
    .append("circle")
    .attr("r", (d) => scaleCircleSUB(d.size))
    .attr("fill", (d) => d.color)
    .call(
      d3
        .drag()
        .on("start", SUBdragStarted)
        .on("drag", SUBdragged)
        .on("end", SUBdragEnded)
    );

  // Add labels for the nodes
  sublabel = sub_svg
    .selectAll("text")
    .data(subgraphData.nodes)
    .enter()
    .append("text")
    .text((d) => d.text.length > 10 ? d.text.slice(0, 20) + " ..." : d.text)
    .style("font-size", "16px")
    .style("fill", "#F1EC91")
    .attr("dy", (d) => scaleCircleSUB(d.size) + 15 + "px") // Vertically center the text
    .attr("text-anchor", "middle"); // Horizontally center the text

  subnode
    .on("mouseover", function (event, d) {
      d3.select(this)
        .transition()
        .duration(200)
        .attr("fill", "white")
        .attr("r", (d) => scaleCircleSUB(d.size) * 1.1);
      tooltip
        .style("display", "none") // TO ENABLE TOOLTIP CHANGE TO "block"
        .style("left", event.pageX + 10 + "px")
        .style("top", event.pageY - 10 + "px")
        .html(`Node: ${d.id}`);
    })
    .on("mouseout", function (event, d) {
      d3.select(this)
        .transition()
        .duration(200)
        .attr("fill", (d) => d.color)
        .attr("r", (d) => scaleCircleSUB(d.size));

      tooltip.style("display", "none");
    })
    .on("click", (event, d) => {
      //sendHttpRequest('GET', d.link).then((getData) => {d.file_text = getData;});
      node_info.style("display", "block").html(getHtmlString(d));
    });
  // Update positions on each tick of the simulation
  subsimulation.on("tick", () => {
    sublink
      .attr("x1", (d) => d.source.x)
      .attr("y1", (d) => d.source.y)
      .attr("x2", (d) => d.target.x)
      .attr("y2", (d) => d.target.y);

    subnode.attr("cx", (d) => d.x).attr("cy", (d) => d.y);

    sublabel.attr("x", (d) => d.x).attr("y", (d) => d.y);
  });
  subsimulation.alphaTarget(0).restart();
}

let subsimulation = d3
  .forceSimulation(subgraphData.nodes)
  .force(
    "link",
    d3
      .forceLink(subgraphData.links)
      .id((d) => d.id)
      .distance(50)
  )
  .force("charge", d3.forceManyBody().strength(-200))
  .force("center", d3.forceCenter(subwidth / 2, subheight / 2))
  .force("edge", SUBrepelFromEdges()); // Custom force to push nodes from edges
// Create lines for the links
let sublink = sub_svg
  .selectAll("line")
  .data(subgraphData.links)
  .enter()
  .append("line")
  .attr("stroke", (d) => d.color);

// Create circles for the nodes, using the size and color fields, with tooltips
let subnode = sub_svg
  .selectAll("circle")
  .data(subgraphData.nodes)
  .enter()
  .append("circle")
  .attr("r", (d) => scaleCircleSUB(d.size))
  .attr("fill", (d) => d.color)
  .call(
    d3.drag().on("start", dragStarted).on("drag", dragged).on("end", dragEnded)
  );

// Add labels for the nodes
let sublabel = sub_svg
  .selectAll("text")
  .data(subgraphData.nodes)
  .enter()
  .append("text")
  .text((d) => d.text)
  .style("font-size", "16px")
  .style("fill", "#F1EC91")
  .attr("dy", (d) => scaleCircleSUB(d.size) + 15 + "px") // Vertically center the text
  .attr("text-anchor", "middle"); // Horizontally center the text

// Drag event handlers
function SUBdragStarted(event, d) {
  if (!event.active) subsimulation.alphaTarget(0.3).restart();
  d.fx = d.x;
  d.fy = d.y;
}

function SUBdragged(event, d) {
  d.fx = event.x;
  d.fy = event.y;
}

function SUBdragEnded(event, d) {
  if (!event.active) subsimulation.alphaTarget(0);
  d.fx = null;
  d.fy = null;
}

function getLinkColors(links) {
  links.forEach((link) => {
    link.color = link.source.color;
  });
}

function getNodeColors(nodes) {
  nodes.forEach((node) => {
    node.color = color_wheel[node.size % color_wheel.length];
  });
}

function scaleCircle(x) {
  return 15 / x + 5;
}
function scaleCircleSUB(x) {
  return 15 / x + 5;
}

function getHtmlString(d) {
  return `<p>Node: ${d.id} <br>
  Node level: ${d.size} <br> </p>
  <p>Node text: ${d.file_text} </p>`;
}
