class ProgressBar extends HTMLElement {
  static get observedAttributes() { return ['budget-amnt', 'current-amnt', "title"]; }

  constructor() {
    super();
    const root = this.attachShadow({mode: "open"});
    const title = document.createElement("h3");
    title.textContent = this.getAttribute("title");
    const subtitle = document.createElement("div");
    subtitle.setAttribute("class", "subtitle");
    subtitle.textContent = `($0 / $0)`;
    const container = document.createElement("div");
    container.setAttribute("class", "container");
    const progressBar = document.createElement("div");
    progressBar.setAttribute("class", "progress");
    
    const style = document.createElement("style");
    style.textContent = `
    .container {
      --radius: 25px;
      border-radius: var(--radius);
      --progress-color: green;
      border: 2px solid black;
      position: relative;
      width: 10rem;
      height: 2rem;
      overflow: hidden;
    }

    h3,
    .subtitle {
      margin-block: 0.5em; 
    }

    .subtitle {
      font-size: 0.875rem;
    }

    .progress {
      color: #ddd;
      text-align: center;
      line-height: 2rem;
      font-weight: bold;
      font-size: 0.625rem;
      position: absolute;
      border-radius: var(--radius);
      left: 0;
      background-color: var(--progress-color);
      width: 0%;
      height: 100%;
      transition: width 1.5s ease-in-out;
    }
    `;
    container.append(progressBar);
    root.append(style, title, subtitle, container);
  }

  setTitle() {
    this.shadowRoot.querySelector("h3").textContent = this.getAttribute("title");
  }
  setSubtitle() {
    const subtitle = this.shadowRoot.querySelector(".subtitle");
    const maxAmnt = parseFloat(this.getAttribute("budget-amnt")).toFixed(2);
    const current = parseFloat(this.getAttribute("current-amnt")).toFixed(2);
    subtitle.textContent = `($${current} / $${maxAmnt})`;
  }

  updateProgressBar() {
    const bar = this.shadowRoot.querySelector(".progress");
    const maxAmnt = parseFloat(this.getAttribute("budget-amnt"));
    const current = parseFloat(this.getAttribute("current-amnt"));
    const percentage = ((current / maxAmnt) * 100).toFixed(2);
    const percent = parseInt(percentage);
    bar.style.color =  percent < 20 || percentage >= 60 ? "#222" : "#ddd";
    bar.textContent = `${percentage}%`;
    bar.style.backgroundColor = percent > 100 ? "red" : percent > 60 ? "orange" : "green";
    bar.style.width = `${Math.min(percentage, 100)}%`;
  }
  
  attributeChangedCallback(attrName, oldVal, newVal) {
    if (attrName === "title") this.setTitle();
    if (attrName === "budget-amnt" || attrName === "current-amnt") setTimeout(this.setSubtitle.bind(this), 1500);
    if (this.getAttribute("budget-amnt") && this.getAttribute("current-amnt")) setTimeout(this.updateProgressBar.bind(this), 1500);
  }
}

  customElements.define("progress-bar", ProgressBar);