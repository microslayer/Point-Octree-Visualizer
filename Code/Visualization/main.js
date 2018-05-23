$(function() {
	$("#showPoints").change(function(){
		var pointsGeometry = scene.getObjectByName("pointsGeometry");
		pointsGeometry.visible = this.checked; 
	})

	$("#showOctree").change(function(){
		var octree = scene.getObjectByName("octree");
		octree.visible = this.checked; 
	})
})