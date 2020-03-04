export default function() {
    const inspector = require("inspector");
    const fs = require("fs");
    const session = new inspector.Session();

    const fd = fs.openSync("profile.heapsnapshot", "w");

    session.connect();

    session.on("HeapProfiler.addHeapSnapshotChunk", m => {
        fs.writeSync(fd, m.params.chunk);
    });

    session.post("HeapProfiler.takeHeapSnapshot", null, (err, r) => {
        console.log("HeapProfiler.takeHeapSnapshot done:", err, r);
        session.disconnect();
        fs.closeSync(fd);
    });
}
