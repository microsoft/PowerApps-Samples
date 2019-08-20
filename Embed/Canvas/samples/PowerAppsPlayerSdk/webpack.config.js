const path = require('path');
module.exports = {  
  mode:"development",
  entry: './src/PlayerSDK.ts',
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  resolve: {
    extensions: [".ts", ".js"]
  },
  devServer:
  {    
    compress: true,
    port: 9000
  },
  output: {
    filename: 'powerappsplayersdksample.js',    
    libraryTarget: "umd",
    path: path.resolve(__dirname, 'dist')
  }
}