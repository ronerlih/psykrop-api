const imageAnalysis = require("../scripts/imageAnalysis");

describe("Initial tests structure, TBD", () => {
   describe("Visual tests testing:", () => {
      it("images list should match expected list", () => {
         const urls = [
            "https://2014.igem.org/wiki/images/a/a7/Sample.png",
            "https://images2.minutemediacdn.com/image/upload/c_crop,h_3236,w_5760,x_0,y_0/f_auto,q_auto,w_1100/v1554700227/shape/mentalfloss/istock-609802128.jpg",
            "https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg",
            "https://qph.fs.quoracdn.net/main-qimg-7e3c3f89920a527c3becb8e312b0a465",
            "https://www.passmark.com/source/img_posts/montest_slide_2.png",
            "https://i.ytimg.com/vi/sr_vL2anfXA/maxresdefault.jpg",
            "https://upload.wikimedia.org/wikipedia/commons/1/16/HDRI_Sample_Scene_Balls_%28JPEG-HDR%29.jpg",
            "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png"
         ];
         const result = imageAnalysis.getTestImages();
         expect(result).toMatchObject(urls);
      });
   });

   describe("Image analysis testing:", () => {
      it("Test error response", async () => {
         const result = await imageAnalysis.analyseImage({}, 0, false);
         expect(result).toBeInstanceOf(Object);
      });
   });
});
