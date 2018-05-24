var scene; 
var pointsGeometry; 

function init() {
	scene = new THREE.Scene({transparent: true});
	var clock = new THREE.Clock(); 

	var directionalLight = getDirectionalLight(5);
	directionalLight.position.x = 13;
	directionalLight.position.y = 10;
	directionalLight.position.z = 10;
	directionalLight.intensity = 2;

	var camera = new THREE.PerspectiveCamera(
		45,
		window.innerWidth/window.innerHeight,
		1,
		10000
	);

	var cameraZRotation = new THREE.Group(); 
	var cameraYPosition = new THREE.Group(); 
	var cameraZPosition = new THREE.Group(); 
	var cameraXRotation = new THREE.Group(); 
	var cameraYRotation = new THREE.Group(); 

	cameraZRotation.add(camera); 
	cameraYPosition.add(cameraZRotation); 
	cameraZPosition.add(cameraYPosition); 
	cameraXRotation.add(cameraZPosition); 
	cameraYRotation.add(cameraXRotation); 
	scene.add(cameraYRotation); 

	cameraXRotation.rotation.x = 0; 
	cameraZPosition.position.y = 0; 
	cameraZPosition.position.z = 0;  

	var octreegroup = new THREE.Object3D();//create an empty container
	octreegroup.name = "octree"; 
	octreegroup.visible = false; 

	for (point of data) {
		var box = getBoundingBox(point.h, point.w, point.d, 'white'); 
		box.position.set(point.x, point.y, point.z); 
		octreegroup.add(box); 
	}

	scene.add(octreegroup); 

	// get center of octreegroup 
	var a = new THREE.Box3().setFromObject(octreegroup)
	var b = a.getBoundingSphere()
	var c = b.center

	// Place camera on x axis
	camera.position.set(50,30,100);
	camera.up = new THREE.Vector3(0,70,0);
	camera.lookAt(c);

	pointsGeometry = new THREE.Geometry();
	pointsGeometry.name = "pointsGeometry"; 

	for ( var i = 0; i < points.length; i++ ) {
		var star = new THREE.Vector3();
		star.x = points[i].x;
		star.y = points[i].y;
		star.z = points[i].z;
		pointsGeometry.vertices.push(star);
	}

	var pointsMaterial = new THREE.PointsMaterial( { color: 'black' } );
	var starField = new THREE.Points( pointsGeometry, pointsMaterial );
	starField.name = "pointsGeometry"; 
	scene.add( starField );

	// renderer
	var renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
	renderer.shadowMap.enabled = true;
	renderer.setSize(window.innerWidth, window.innerHeight);
  	renderer.setClearColor(0x000000, 0.0); // transparent
  	renderer.autoClear = false;
	document.getElementById('webgl').appendChild(renderer.domElement);

	// controls
	var controls; 
	controls = new THREE.OrbitControls(camera, renderer.domElement );
	controls.enableZoom = true;
	controls.enablePan = true;
	controls.enableRotate = true;
	controls.enableDamping = true;
	controls.dampingFactor = 0.25;
	controls.screenSpacePanning = true;
	controls.minDistance = 5;
	controls.maxDistance = 50000; 

	var axisHelper = new THREE.AxisHelper( 5 );
	scene.add( axisHelper );

	update(renderer, scene, camera, controls, clock);

	return scene;
}

function getBoundingBox(w, h, d, color) {
	var geometry = new THREE.BoxGeometry(w, h, d);
	var material = new THREE.MeshBasicMaterial({
        color: color, 
        opacity: 0.05, 
        transparent: true
    	}); 

	geometry.center(); 
    var mesh = new THREE.Mesh(geometry, material);

	var geometry = new THREE.BoxGeometry(w, h, d);
	geometry.center(); 
	var geo = new THREE.EdgesGeometry( geometry ); 
	var mat = new THREE.LineBasicMaterial( { color: color, linewidth: 4 } );
	var wireframe = new THREE.LineSegments( geo, mat );

	var group = new THREE.Object3D();//create an empty container
	group.add(mesh); 
	group.add(wireframe); 
	return group; 
}


function getSphere(size) {
	var geometry = new THREE.SphereGeometry(size, 24, 24);
	var material = new THREE.MeshBasicMaterial({
		color: 'rgb(255, 255, 255)'
	});
	var mesh = new THREE.Mesh(
		geometry,
		material 
		);

	return mesh;
}

function getPointLight(intensity) {
	var light = new THREE.PointLight(0xffffff, intensity);
	light.castShadow = true;

	return light;
}

function getSpotLight(intensity) {
	var light = new THREE.SpotLight(0xffffff, intensity);
	light.castShadow = true;

	light.shadow.bias = 0.001;
	light.shadow.mapSize.width = 2048;
	light.shadow.mapSize.height = 2048;

	return light;
}

function getDirectionalLight(intensity) {
	var light = new THREE.DirectionalLight(0xffffff, intensity);
	light.castShadow = true;
	light.shadow.camera.left = -40;
	light.shadow.camera.bottom = -40;
	light.shadow.camera.right = 40;
	light.shadow.camera.top = 40;
	light.shadow.mapSize.width = 4096; 
	light.shadow.mapSize.height = 4096; 
	return light;
}

function getCenterPoint(mesh) {
    var geometry = mesh.geometry;
    geometry.computeBoundingBox();   
    center = geometry.boundingBox.getCenter();
    mesh.localToWorld( center );
    return center;
}

function update(renderer, scene, camera, controls, clock) {
	renderer.render(
		scene,
		camera
	);

	var timeElapsed = clock.getElapsedTime(); 

	controls.update(); 

	requestAnimationFrame(function() {
		update(renderer, scene, camera, controls, clock);
		controls.update();
	})
}

var scene = init();