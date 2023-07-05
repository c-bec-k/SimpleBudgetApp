const State = (initialVal) => {
  let currentVal = initialVal;
  const observers = [];
  return {
    val: () => currentVal,
    update: (fn) => {
      let oldVal = currentVal;
      let newVal = fn(currentVal);
      if (oldVal.entries() !== newVal.entries()) {
        currentVal = newVal;
        observers.forEach( (observer) => observer(newVal));
      }
    },
    observe: (fn) => observers.push(fn),
  }
}

const myState = State(new Map());

// myState.observe(() => document.querySelector(".container").innerHTML = "");
myState.observe( (map) => {
  const container = document.querySelector(".container");
  for (const [title, {max, current}] of map) {
    if (document.querySelector(`[title=${title}]`)) continue;
    const el = document.createElement("progress-bar");
    el.setAttribute("title", title);
    el.setAttribute("budget-amnt", max);
    el.setAttribute("current-amnt", current);
    container.append(el);
  }
});

setTimeout(() =>
myState.update((map) => map.set("Groceries", {max: 500.00,current: 30.48})
  .set("Gas", {max: 250,current: 62.99})
  .set("Movies", {max: 32,current: 34.74})
  .set("Coffee", {max: 80,current: 58.42})
  ), 1000);

setTimeout(() => myState.update( map => map.set("Insurance", {max: 295.19, current: 62.99})), 5000);