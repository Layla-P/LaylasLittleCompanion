let world;
const { Engine, Render, Runner, World, Bodies } = Matter;


function Booper(action) {
	world = document.querySelector(".boops")
	console.log(world);

        //let count = action === "super" ? 5 : 13;
        //let image = action === "waffle" ? "/images/waffle.png" : "/images/destructopup-112.png";
	triggerRain(5, "/images/waffle.png");   
    
};

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


const triggerRain = (count, image) => {

    for (let i = 0; i < count; i++) {
        World.add(engine.world, [createBall(image)])
    }
};


