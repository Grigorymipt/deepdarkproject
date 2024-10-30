// PART WITH GETTING FILE ON CLICK
const getMindMapButton = document.getElementById("download-button");

// First attempt on sending requests
const sendHttpRequest = (method, url, data) => {
  return fetch(url, {
    method: method,
    body: JSON.stringify(data),
    headers: data ? { "Content-Type": "application/json" } : {},
  }).then((response) => {
    if (response.status >= 400) {
      return response.json().then((errResData) => {
        const error = new Error("Smth went wrong!");
        error.data = errResData;
        throw error;
      });
    }
    return response.text();
  });
};
