'use-strict'
const world = document.querySelector(".boops");
const { Engine, Render, Runner, World, Bodies } = Matter;

export function Booper(action) {
    let count = action === "super" ? 50 : 13;
    let image = action === "waffle" ? "images/waffle.png" : "images/destructopup-112.png";
    triggerRain(count, image);
};

const engine = Engine.create();
const runner = Runner.create();
const render = Render.create({
    canvas: world,
    engine: engine,
    options: {
        width: 1280,
        height: 720,
        background: "transparent",
        wireframes: false
    }
});

const boundaryOptions = {
    isStatic: true,
    render: {
        fillStyle: "transparent",
        strokeStyle: "transparent"
    }
};
const ground = Bodies.rectangle(640, 720, 1300, 4, boundaryOptions);
const leftWall = Bodies.rectangle(0, 360, 4, 740, boundaryOptions);
const rightWall = Bodies.rectangle(1280, 360, 4, 800, boundaryOptions);
Render.run(render);
Runner.run(runner, engine);

World.add(engine.world, [ground, leftWall, rightWall]);

function createBall(image) {
    const ball = Bodies.circle(Math.round(Math.random() * 1280), -30, 25, {
        angle: Math.PI * (Math.random() * 2 - 1),
        friction: 0.001,
        frictionAir: 0.01,
		restitution: 0.8,
        render: {
            sprite: {
                texture: image
            }
        }
    });
	
    setTimeout(() => {
        World.remove(engine.world, ball);
    }, 30000);

    return ball;
}


const triggerRain = (count, image) => {

    for (let i = 0; i < count; i++) {
        World.add(engine.world, [createBall(image)])
    }
};



// Returns random whole number
function Rand(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

// Returns random float with two decimals
function RandFloat(min, max) {
    let a = Math.random() * (max - min) + min;

    a = a.toFixed(2);

    return a;
}
